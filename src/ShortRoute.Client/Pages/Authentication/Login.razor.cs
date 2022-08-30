using ShortRoute.Client.Components.Common;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ShortRoute.Client.Infrastructure.Auth;
using ShortRoute.Client.Infrastructure.Auth.Enums;
using ShortRoute.Contracts.Commands.Authentication;

namespace ShortRoute.Client.Pages.Authentication;

public partial class Login
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    public IAuthenticationService AuthService { get; set; } = default!;

    private CustomValidation? _customValidation;

    public bool BusySubmitting { get; set; }

    private readonly AuthenticateCommand _tokenRequest = new();
    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected override async Task OnInitializedAsync()
    {
        if (AuthService.ProviderType == AuthProvider.AzureAd)
        {
            AuthService.NavigateToExternalLogin(Navigation.Uri);
            return;
        }

        var authState = await AuthState;
        if (authState.User.Identity?.IsAuthenticated is true)
        {
            Navigation.NavigateTo("/");
        }
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

    private void FillAdministratorCredentials()
    {
        //_tokenRequest.Email = MultitenancyConstants.Root.EmailAddress;
        //_tokenRequest.Password = MultitenancyConstants.DefaultPassword;
        //TenantId = MultitenancyConstants.Root.Id;
    }

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        if (await ApiHelper.ExecuteClientCall(
            () => AuthService.LoginAsync(_tokenRequest),
            Snackbar,
            _customValidation))
        {
            Snackbar.Add($"Logged in as {_tokenRequest.User}", Severity.Info);
        }

        BusySubmitting = false;
    }
}