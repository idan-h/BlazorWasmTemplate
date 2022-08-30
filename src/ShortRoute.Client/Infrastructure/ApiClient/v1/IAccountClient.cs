using Refit;
using ShortRoute.Contracts.Commands.App.Account;

namespace ShortRoute.Client.Infrastructure.ApiClient.v1;

public interface IAccountClient
{
    /// <summary>
    /// Creates a new account for the client. AS OF NOW DOES NOTHING
    /// </summary>
    [Post("/api/v1/account/sign-up")]
    public Task<ApiResponse<object>> CreateAccount();

    /// <summary>
    /// Sends an email with password restoration link
    /// </summary>
    [Post("/api/v1/account/forgot-password")]
    public Task<ApiResponse<object>> ForgotPassword(ForgotPasswordCommand forgotPassword);

    /// <summary>
    /// Changes the password to a new password
    /// </summary>
    [Post("/api/v1/account/change-password")]
    public Task<ApiResponse<object>> ChangePassword(ChangePasswordCommand changePassword);
}
