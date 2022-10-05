using ShortRoute.Client.Components.EntityTable;
using ShortRoute.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Contracts.Enums;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Client.Models.Generic;
using ShortRoute.Client.Models.Users;
using ShortRoute.Contracts.Commands.Authentication.Roles;

namespace ShortRoute.Client.Pages.Identity.Roles;

public partial class Roles
{
    [Inject]
    private IRolesClient RolesClient { get; set; } = default!;

    protected EntityClientTableContext<RoleDto, string, CreateUpdateRoleModel, CreateRoleCommand, UpdateRoleCommand> Context { get; set; } = default!;

    protected override void OnInitialized()
    {
        Context = new(
            entityName: L["Role"],
            entityNamePlural: L["Roles"],
            searchPermission: Permissions.ReadRoles,
            createPermission: Permissions.CreateRoles,
            updatePermission: Permissions.UpdateRoles,
            deletePermission: Permissions.DeleteRoles,
            fields: new()
            {
                new(role => role.RoleName, L["Name"]),
                new(role => role.Description, L["Description"])
            },
            idFunc: role => role.RoleName,
            loadDataFunc: async () => (await RolesClient.RolesGetList()).Roles.ToList(),
            searchFunc: (searchString, role) =>
                string.IsNullOrWhiteSpace(searchString)
                    || role.RoleName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || role.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async role => await RolesClient.RolesCreate(role),
            updateFunc: async (_, role) => await RolesClient.RolesUpdate(role),
            deleteFunc: async name => await RolesClient.RolesDelete(name!));
    }

    private void ManagePermissions(string? roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));
        Navigation.NavigateTo($"/roles/{roleId}/permissions");
    }
}