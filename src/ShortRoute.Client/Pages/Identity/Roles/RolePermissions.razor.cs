using System.Security.Claims;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Enums;
using ShortRoute.Client.Models.Roles;

namespace ShortRoute.Client.Pages.Identity.Roles;

public partial class RolePermissions
{
    [Inject]
    protected IRolesClient RolesClient { get; set; } = default!;

    [Parameter]
    public bool IsReadonly { get; set; }

    [Parameter]
    public EventCallback<List<string>> ValueChanged { get; set; }

    [Parameter]
    public List<string> Value
    {
        get => _value;
        set
        {
            if (value == _value)
            {
                return;
            }

            _value = value;

            ValueChanged.InvokeAsync(_value);
        }
    }
    private List<string> _value = new();

    private Dictionary<string, List<PermissionModel>> _groupedRoleClaims = default!;

    private string _searchString = string.Empty;

    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var response = await ApiHelper.ExecuteClientCall(() => RolesClient.PermissionsGetList(), Snackbar);

        if (response is not null)
        {
            _groupedRoleClaims = response.Permissions
                .GroupBy(p => p.GroupName!)
                .ToDictionary(g => g.Key, g => g.Select(p =>
                {
                    return new PermissionModel(p, Value.Contains(p.PermissionName!), OnPermissionStatusChanged);
                }).ToList());
        }

        _loaded = true;
    }

    private void OnPermissionStatusChanged()
    {
        Value = _groupedRoleClaims.Values
            .SelectMany(a => a)
            .Where(a => a.Enabled)
            .Select(x => x.Dto.PermissionName!)
            .ToList();
    }

    private Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
            return Color.Error;

        if (selected == all)
            return Color.Success;

        return Color.Info;
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
