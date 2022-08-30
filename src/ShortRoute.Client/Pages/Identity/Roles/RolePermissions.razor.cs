using System.Security.Claims;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Client.Models;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Auth;

namespace ShortRoute.Client.Pages.Identity.Roles;

public partial class RolePermissions
{
    [Parameter]
    public string Id { get; set; } = default!; // from route
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IRolesClient RolesClient { get; set; } = default!;

    private Dictionary<string, List<PermissionModel>> _groupedRoleClaims = default!;

    public string _title = string.Empty;
    public string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditRoleClaims;
    private bool _canSearchRoleClaims;
    private bool _loaded;


    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canEditRoleClaims = await AuthService.HasPermissionAsync(state.User, Permissions.RoleChange);
        _canSearchRoleClaims = await AuthService.HasPermissionAsync(state.User, Permissions.RoleRead);

        var role = await ApiHelper.ExecuteClientCall(() => RolesClient.RolesGetSingle(Id), Snackbar);
        var permissions = await ApiHelper.ExecuteClientCall(() => RolesClient.PermissionsGetList(), Snackbar);

        if (permissions is not null && role is not null && role.PermissionNames?.Any() is true)
        {
            _title = string.Format(L["{0} Permissions"], role.RoleName);
            _description = string.Format(L["Manage {0} Role Permissions"], role.RoleName);

            _groupedRoleClaims = permissions
                .GroupBy(p => p.GroupName!)
                .ToDictionary(g => g.Key, g => g.Select(p =>
                {
                    return new PermissionModel(p, role.PermissionNames.Contains(p.PermissionName!));
                }).ToList());
        }

        _loaded = true;
    }

    private Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
            return Color.Error;

        if (selected == all)
            return Color.Success;

        return Color.Info;
    }

    private async Task SaveAsync()
    {
        var allPermissions = _groupedRoleClaims.Values.SelectMany(a => a);
        var selectedPermissions = allPermissions.Where(a => a.Enabled);
        var dto = new RoleDto()
        {
            RoleName = Id,
            PermissionNames = selectedPermissions.Where(x => x.Enabled).Select(x => x.Dto.PermissionName).ToList()!,
        };

        if (await ApiHelper.ExecuteClientCall(
            () => RolesClient.RolesUpdate(dto),
            Snackbar,
            successMessage: L["Updated Permissions."]))
        {
            Navigation.NavigateTo("/roles");
        }
    }

    private bool Search(PermissionModel permission)
    {
        var permissionDto = permission.Dto;

        return string.IsNullOrWhiteSpace(_searchString)
            || permissionDto.PermissionName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true
            || permissionDto.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true
            || permissionDto.GroupName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
    }
}
