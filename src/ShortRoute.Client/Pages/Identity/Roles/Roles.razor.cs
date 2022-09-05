using ShortRoute.Client.Components.EntityTable;
using ShortRoute.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Contracts.Auth;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Pages.Identity.Roles;

public partial class Roles
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    private IRolesClient RolesClient { get; set; } = default!;

    protected EntityClientTableContext<RoleDto, string?, RoleDto> Context { get; set; } = default!;

    private bool _canViewRoleClaims;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canViewRoleClaims = await AuthService.HasPermissionAsync(state.User, Permissions.RoleRead);

        Context = new(
            entityName: L["Role"],
            entityNamePlural: L["Roles"],
            searchPermission: Permissions.RoleRead,
            fields: new()
            {
                new(role => role.RoleName, L["Name"]),
                new(role => role.Description, L["Description"])
            },
            idFunc: role => role.RoleName,
            loadDataFunc: async () => (await RolesClient.RolesGetList()).ToList(),
            searchFunc: (searchString, role) =>
                string.IsNullOrWhiteSpace(searchString)
                    || role.RoleName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || role.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async role => await RolesClient.RolesCreate(role),
            updateFunc: async (_, role) => await RolesClient.RolesUpdate(role),
            deleteFunc: async name => await RolesClient.RolesDelete(name!),
            hasExtraActionsFunc: () => _canViewRoleClaims,
            canUpdateEntityFunc: e => true,
            canDeleteEntityFunc: e => true);
    }

    private void ManagePermissions(string? roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));
        Navigation.NavigateTo($"/roles/{roleId}/permissions");
    }
}