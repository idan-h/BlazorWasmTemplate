using ShortRoute.Client.Components.Common;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ShortRoute.Client.Pages.Authentication;

public partial class SelfRegister
{
    //private readonly CreateUserRequest _createUserRequest = new();
    private bool BusySubmitting { get; set; }

    //[Inject]
    //private IUsersClient UsersClient { get; set; } = default!;

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        //string? sucessMessage = await ApiHelper.ExecuteClientCall(
        //    () => UsersClient.SelfRegisterAsync(Tenant, _createUserRequest),
        //    Snackbar);

        //if (sucessMessage != null)
        //{
        //    Snackbar.Add(sucessMessage, Severity.Info);
        //    Navigation.NavigateTo("/login");
        //}

        BusySubmitting = false;
    }

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }
}