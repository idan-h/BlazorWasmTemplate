using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Contracts.Auth;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Client.Models.Roles;

namespace ShortRoute.Client.Pages.Identity.Users;

public partial class UserRoles
{
    [Parameter]
    public string? Id { get; set; }
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IUsersClient UsersClient { get; set; } = default!;
    [Inject]
    protected IRolesClient RolesClient { get; set; } = default!;

    private UpdateUserCommand _user = default!;
    private List<RoleModel> _userRolesList = default!;

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditUsers;
    private bool _canSearchRoles;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canEditUsers = await AuthService.HasPermissionAsync(state.User, Permissions.UpdateUsers);
        _canSearchRoles = await AuthService.HasPermissionAsync(state.User, Permissions.UpdateUsers);

        if (await ApiHelper.ExecuteClientCall(
                () => UsersClient.UsersGetSingle(Id!), Snackbar)
            is UserDto user)
        {
            _user = new()
            {
                Id = user.Id!,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                RoleNames = user.RoleNames,
            };
            _title = user.FullName;
            _description = string.Format(L["Manage {0}'s Roles"], user.FullName);

            if (await ApiHelper.ExecuteClientCall(() => RolesClient.RolesGetList(), Snackbar)
                is ICollection<RoleDto> response)
            {
                _userRolesList = user.RoleNames.Select(r => new RoleModel(response.First(n => n.RoleName == r), true)).ToList();
            }
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        _user.RoleNames = _userRolesList.Select(r => r.Dto.RoleName);
        if (await ApiHelper.ExecuteClientCall(() => UsersClient.UsersUpdate(_user), Snackbar, successMessage: L["Updated User Roles."]))
        {
            Navigation.NavigateTo("/users");
        }
    }

    private bool Search(RoleModel userRole) =>
        string.IsNullOrWhiteSpace(_searchString)
            || userRole.Dto.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}