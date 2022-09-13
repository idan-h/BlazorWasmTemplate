using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Infrastructure.Auth.Enums;
using ShortRoute.Contracts.Commands.Authentication.Login;

namespace ShortRoute.Client.Infrastructure.Auth;

public interface IAuthenticationService
{
    AuthProvider ProviderType { get; }

    void NavigateToExternalLogin(string returnUrl);

    Task<bool> LoginAsync(AuthenticateCommand command);

    Task LogoutAsync();

    Task ReLoginAsync(string returnUrl);
}