using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Commands.Authentication;
using ShortRoute.Contracts.Commands.Authentication.Login;
using ShortRoute.Contracts.Endpoints;
using ShortRoute.Contracts.Responses.Authentication;
using ShortRoute.Contracts.Responses.Authentication.Login;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IAuthClient : IApiClient
{
    /// <summary>
    /// Authenticates the user by username and password
    /// </summary>
    [Post("/api/v1/auth")]
    public Task<AuthenticateResponse> Authenticate(AuthenticateCommand authenticate);

    /// <summary>
    /// Refreshes the authentication by an expired valid token and a refresh token
    /// </summary>
    [Post("/api/v1/refresh-auth")]
    public Task<AuthenticateResponse> RefreshAuthentication(RefreshAuthenticationCommand refreshAuthentication);

    /// <summary>
    /// Invalidates the last refresh token
    /// </summary>
    [Post("/api/v1/logout")]
    public Task Logout();

    /// <summary>
    /// A list of all the current logged in user permissions
    /// </summary>
    [Post("/api/v1/user-permissions")]
    public Task<string[]> UserPermissions();

    /// <summary>
    /// The authentication for hangfire
    /// </summary>
    [Post("/hangfire-auth")]
    public Task<string> HangfireAuth();
}
