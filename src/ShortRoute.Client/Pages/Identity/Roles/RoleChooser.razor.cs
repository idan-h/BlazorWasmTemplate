using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Contracts.Enums;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Client.Models.Roles;
using ShortRoute.Contracts.Responses.Authentication.Users;

namespace ShortRoute.Client.Pages.Identity.Roles;

public partial class RoleChooser
{
    private List<RoleModel> _userRolesList = default!;

    private string _searchString = string.Empty;

    private bool _loaded;

    [Parameter]
    public bool IsReadonly { get; set; }

    [Parameter]
    public Func<Task<IEnumerable<RoleDto>>> GetRoleListFunc { get; set; } = default!;

    [Parameter]
    public EventCallback<IEnumerable<string>> ValueChanged { get; set; }

    [Parameter]
    public IEnumerable<string> Value
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
    private IEnumerable<string> _value = Enumerable.Empty<string>();

    protected override async Task OnInitializedAsync()
    {
        await Refresh();
        _loaded = true;
    }

    public async Task Refresh()
    {
        var roles = await GetRoleListFunc();

        _userRolesList = roles
            .Select(r => new RoleModel(r, Value.Any(n => n == r.RoleName), OnRoleStatusChanged))
            .ToList();
    }

    private void OnRoleStatusChanged()
    {
        Value = _userRolesList.Where(r => r.Enabled).Select(r => r.Dto.RoleName);
    }

    private bool Search(RoleModel userRole) =>
        string.IsNullOrWhiteSpace(_searchString)
            || userRole.Dto.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}