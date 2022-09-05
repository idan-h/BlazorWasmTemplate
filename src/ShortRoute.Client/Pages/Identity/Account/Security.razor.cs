using ShortRoute.Client.Components.Common;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShortRoute.Contracts.Commands.App.Account;
using ShortRoute.Client.Infrastructure.ApiClient.v1;

namespace ShortRoute.Client.Pages.Identity.Account;

public partial class Security
{
    [Inject]
    public IAccountClient AccountClient { get; set; } = default!;

    private readonly ChangePasswordCommand _passwordModel = new();
    private string _confirmedPassword;

    private CustomValidation? _customValidation;

    private async Task ChangePasswordAsync()
    {
        if (_passwordModel.NewPassword != _confirmedPassword)
        {
            _customValidation?.DisplayErrors(new Dictionary<string, ICollection<string>>
            {
                [nameof(_passwordModel.NewPassword)] = new[]
                {
                    L["Passwords don't match"].Value,
                }
            });
            return;
        }

        if (await ApiHelper.ExecuteClientCall(
            () => AccountClient.ChangePassword(_passwordModel),
            Snackbar,
            L["Password Changed!"]))
        {
            _passwordModel.CurrentPassword = string.Empty;
            _passwordModel.NewPassword = string.Empty;
            _confirmedPassword = string.Empty;
        }
    }

    private bool _currentPasswordVisibility;
    private InputType _currentPasswordInput = InputType.Password;
    private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _newPasswordVisibility;
    private InputType _newPasswordInput = InputType.Password;
    private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordVisibility(bool newPassword)
    {
        if (newPassword)
        {
            if (_newPasswordVisibility)
            {
                _newPasswordVisibility = false;
                _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _newPasswordInput = InputType.Password;
            }
            else
            {
                _newPasswordVisibility = true;
                _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                _newPasswordInput = InputType.Text;
            }
        }
        else
        {
            if (_currentPasswordVisibility)
            {
                _currentPasswordVisibility = false;
                _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _currentPasswordInput = InputType.Password;
            }
            else
            {
                _currentPasswordVisibility = true;
                _currentPasswordInputIcon = Icons.Material.Filled.Visibility;
                _currentPasswordInput = InputType.Text;
            }
        }
    }
}