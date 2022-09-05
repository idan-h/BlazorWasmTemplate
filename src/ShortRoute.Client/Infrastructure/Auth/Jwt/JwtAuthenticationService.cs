using ShortRoute.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Commands.Authentication;
using ShortRoute.Contracts.Responses.Authentication;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Client.Infrastructure.Auth.Enums;
using ShortRoute.Contracts.Auth;
using Newtonsoft.Json;
using System.Text;

namespace ShortRoute.Client.Infrastructure.Auth.Jwt;

public class JwtAuthenticationService : AuthenticationStateProvider, IAuthenticationService, IAccessTokenProvider
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly ILocalStorageService _localStorage;
    private readonly IAuthClient _authClient;
    private readonly NavigationManager _navigation;

    public AuthProvider ProviderType => AuthProvider.Jwt;

    public JwtAuthenticationService(ILocalStorageService localStorage, IAuthClient authClient, NavigationManager navigation)
    {
        _localStorage = localStorage;
        _authClient = authClient;
        _navigation = navigation;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string cachedToken = await GetCachedAuthTokenAsync();
        if (string.IsNullOrWhiteSpace(cachedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Generate claimsIdentity from cached token
        var claimsIdentity = new ClaimsIdentity(GetClaimsFromJwt(cachedToken), "jwt");

        // Add cached permissions as claims
        if (await GetCachedPermissionsAsync() is List<string> cachedPermissions)
        {
            claimsIdentity.AddClaims(cachedPermissions.Select(p => new Claim(AuthClaimTypes.Permission, p)));
        }

        return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
    }

    public void NavigateToExternalLogin(string returnUrl) =>
        throw new NotImplementedException();

    public async Task<bool> LoginAsync(AuthenticateCommand command)
    {
        var authResponse = await _authClient.Authenticate(command);

        if (string.IsNullOrWhiteSpace(authResponse.Token))
        {
            return false;
        }

        await CacheAuthTokens(authResponse);

        // Get permissions for the current user and add them to the cache
        string[] permissions;
        try
        {
            permissions = await _authClient.UserPermissions();
        }
        catch
        {
            return false;
        }

        if (permissions.Contains("AccessAll"))
        {
            permissions = Permissions.All;
        }

        await CachePermissions(permissions);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return true;
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _authClient.Logout();
        }
        catch { }
        await ClearCacheAsync();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        _navigation.NavigateTo("/login");
    }

    public async Task ReLoginAsync(string returnUrl)
    {
        await LogoutAsync();
        _navigation.NavigateTo(returnUrl);
    }

    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var authState = await GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated is not true)
        {
            return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login");
        }

        // We make sure the access token is only refreshed by one thread at a time. The other ones have to wait.
        await _semaphore.WaitAsync();
        try
        {
            var token = await GetCachedAuthTokenAsync();
            var tokenExpiration = await GetCachedAuthTokenExpirationAsync();

            var diff = DateTimeOffset.FromUnixTimeSeconds(tokenExpiration) - DateTimeOffset.UtcNow;
            if (diff.TotalSeconds <= 10)
            {
                // string? refreshToken = await GetCachedRefreshTokenAsync();
                (bool succeeded, var response) = await TryRefreshTokenAsync(new() { Token = token });
                if (!succeeded)
                {
                    return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login");
                }

                token = response?.Token;
            }

            return new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken() { Value = token }, string.Empty);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) =>
        RequestAccessToken();

    private async Task<(bool Succeeded, AuthenticateResponse? Token)> TryRefreshTokenAsync(RefreshAuthenticationCommand command)
    {
        var authState = await GetAuthenticationStateAsync();
        var userId = authState.User.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("Can't refresh token when user is not logged in!");
        }

        var refreshTokenExpiration = await GetCachedRefreshTokenExpirationAsync();
        var diff = DateTimeOffset.FromUnixTimeSeconds(refreshTokenExpiration) - DateTimeOffset.UtcNow;
        if (diff.TotalMinutes <= 1)
        {
            return (false, null);
        }

        try
        {
            var authData = await _authClient.RefreshAuthentication(command);
            await CacheAuthTokens(authData);

            return (true, authData);
        }
        catch
        {
            return (false, null);
        }
    }

    private async ValueTask CacheAuthTokens(AuthenticateResponse response)
    {
        await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, response.Token);
        //await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, response.RefreshToken);
        await _localStorage.SetItemAsync(StorageConstants.Local.AuthTokenExpiration, response.TokenExpiration);
        await _localStorage.SetItemAsync(StorageConstants.Local.RefreshTokenExpiration, response.RefreshTokenExpiration);
    }

    private ValueTask CachePermissions(ICollection<string> permissions) =>
        _localStorage.SetItemAsync(StorageConstants.Local.Permissions, permissions);

    private async Task ClearCacheAsync()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.Permissions);
    }

    private ValueTask<string> GetCachedAuthTokenAsync() =>
        _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);

    private ValueTask<long> GetCachedAuthTokenExpirationAsync() =>
        _localStorage.GetItemAsync<long>(StorageConstants.Local.AuthTokenExpiration);

    private ValueTask<string> GetCachedRefreshTokenAsync() =>
        _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

    private ValueTask<long> GetCachedRefreshTokenExpirationAsync() =>
        _localStorage.GetItemAsync<long>(StorageConstants.Local.RefreshTokenExpiration);

    private ValueTask<ICollection<string>> GetCachedPermissionsAsync() =>
        _localStorage.GetItemAsync<ICollection<string>>(StorageConstants.Local.Permissions);

    private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(jsonBytes));

        if (keyValuePairs is not null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles is not null)
            {
                string? rolesString = roles.ToString();
                if (!string.IsNullOrEmpty(rolesString))
                {
                    if (rolesString.Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonConvert.DeserializeObject<string[]>(rolesString);

                        if (parsedRoles is not null)
                        {
                            claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesString));
                    }
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
        }

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
        string base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }
}