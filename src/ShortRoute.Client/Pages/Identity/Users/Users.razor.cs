using ShortRoute.Client.Components.EntityTable;
using ShortRoute.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Contracts.Enums;
using ShortRoute.Contracts.Queries.Common;
using ShortRoute.Contracts.Queries.Authentication.Users;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Client.Models.Users;
using ShortRoute.Client.Mappings.Users;
using ShortRoute.Client.Shared;
using ShortRoute.Contracts.Responses.Authentication.Users;
using ShortRoute.Client.Pages.Identity.Roles;
using ShortRoute.Contracts.Extensions;

namespace ShortRoute.Client.Pages.Identity.Users;

public partial class Users
{
    [Parameter]
    public string? TenantId { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    protected IUsersClient UsersClient { get; set; } = default!;

    protected EntityServerTableContext<UserDto, string, CreateUpdateUserModel, CreateUserCommand, UpdateUserCommand> Context { get; set; } = default!;

    private bool _canUpdate;
    private bool _canChooseTenant;

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private RoleChooser _userRoles = default!;

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthState).User;
        _canUpdate = await AuthService.HasPermissionAsync(user, Permissions.UpdateUsers);
        _canChooseTenant = string.IsNullOrEmpty(user.GetTenant());

        Context = new(
            entityName: L["User"],
            entityNamePlural: L["Users"],
            searchPermission: Permissions.ReadUsers,
            createPermission: Permissions.CreateUsers,
            updatePermission: Permissions.UpdateUsers,
            deletePermission: Permissions.DeleteUsers,
            exportPermission: string.Empty,
            fields: new()
            {
                new(user => user.FullName, L["FullName"]),
                new(user => user.Email, L["Email"]),
                new(user => user.TenantName, L["Tenant"]),
                new(user => user.IsActive, L["Active"], Type: typeof(bool)),
                new(user => user.IsTenantAdmin, L["IsAdmin"], Type: typeof(bool)),
            },
            idFunc: u => u.Id!,
            searchFunc: async filter =>
            {
                var response = await UsersClient.UsersGetList(
                    filter?.Pagination?.ToString(),
                    filter?.Filter?.Text,
                    filter?.Sort?.ToString(),
                    TenantId);

                return new PaginationResponse<UserDto>
                {
                    Data = response.Users.ToList(),
                    TotalCount = 0,
                };
            },
            getDefaultsFunc: null,
            //getDetailsFunc: async id => (await UsersClient.UsersGetSingle(id)).ToModel(),
            createFunc: async command => await UsersClient.UsersCreate(command),
            updateFunc: async (_, command) => await UsersClient.UsersUpdate(command),
            deleteFunc: async id => await UsersClient.UsersDelete(id),

            hasExtraActionsFunc: () => _canUpdate,
            enableSort: false);
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
        Context.AddEditModal.ForceRender();
    }

    private void TenantsAutocompleteValueChanged(string tenantId, CreateUpdateUserModel model)
    {
        if (tenantId == model.TenantId)
        {
            return;
        }

        model.TenantId = tenantId;
        _ = _userRoles?.Refresh();
    }

    private async Task ChangeUserActivation(string id, bool isActive, Action onSuccess)
    {
        if (await ApiHelper.ExecuteClientCall(() => UsersClient.UsersChangeActive(new ChangeUserActiveCommand
        {
            UserId = id,
            IsActive = !isActive
        }), Snackbar))
        {
            onSuccess.Invoke();
        }
    }

    private async Task<IEnumerable<RoleDto>> GetRolesToUser(string? tenantId)
    {
        if (await ApiHelper.ExecuteClientCall(() => UsersClient.UsersRolesGet(tenantId), Snackbar)
                is GetUsersRolesResponse response)
        {
            return response.Roles;
        }

        return Enumerable.Empty<RoleDto>();
    }
}