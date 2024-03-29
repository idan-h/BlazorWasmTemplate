﻿using ShortRoute.Client.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShortRoute.Client.Infrastructure.Auth.Extensions;
using ShortRoute.Contracts.Enums;

namespace ShortRoute.Client.Shared;

public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private string? _hangfireUrl;
    private bool _canViewHangfire;
    private bool _canViewDashboard;
    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewProducts;
    private bool _canViewBrands;
    private bool _canViewTenants;
    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;

    protected override async Task OnParametersSetAsync()
    {
        _hangfireUrl = Config[ConfigNames.ApiBaseUrl] + "jobs";
        var user = (await AuthState).User;
        _canViewHangfire = await AuthService.HasPermissionAsync(user, Permissions.Hangfire);
        _canViewDashboard = true;// await AuthService.HasPermissionAsync(user, Permissions.);
        _canViewRoles = await AuthService.HasPermissionAsync(user, Permissions.ReadRoles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, Permissions.ReadUsers);
        _canViewProducts = true;// await AuthService.HasPermissionAsync(user, Permissions.);
        _canViewBrands = true;// await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Brands);
        _canViewTenants = await AuthService.HasPermissionAsync(user, Permissions.ReadTenants);
    }
}