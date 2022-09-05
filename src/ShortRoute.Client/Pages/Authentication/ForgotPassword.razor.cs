using ShortRoute.Client.Components.Common;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Components;
using ShortRoute.Contracts.Commands.App.Account;
using ShortRoute.Client.Infrastructure.ApiClient.v1;

namespace ShortRoute.Client.Pages.Authentication;

public partial class ForgotPassword
{
    private readonly ForgotPasswordCommand _forgotPasswordRequest = new();
    private CustomValidation? _customValidation;
    private bool BusySubmitting { get; set; }

    [Inject]
    private IAccountClient AccountClient { get; set; } = default!;

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ApiHelper.ExecuteClientCall(
            () => AccountClient.ForgotPassword(_forgotPasswordRequest),
            Snackbar);

        BusySubmitting = false;
    }
}