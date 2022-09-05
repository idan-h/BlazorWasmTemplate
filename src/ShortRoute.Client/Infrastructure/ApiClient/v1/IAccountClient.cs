using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Contracts.Commands.App.Account;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IAccountClient : IApiClient
{
    /// <summary>
    /// Creates a new account for the client. AS OF NOW DOES NOTHING
    /// </summary>
    [Post("/api/v1/account/sign-up")]
    public Task CreateAccount();

    /// <summary>
    /// Sends an email with password restoration link
    /// </summary>
    [Post("/api/v1/account/forgot-password")]
    public Task ForgotPassword(ForgotPasswordCommand forgotPassword);

    /// <summary>
    /// Changes the password to a new password
    /// </summary>
    [Post("/api/v1/account/change-password")]
    public Task ChangePassword(ChangePasswordCommand changePassword);
}
