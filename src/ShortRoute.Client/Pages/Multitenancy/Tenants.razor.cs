using ShortRoute.Client.Components.EntityTable;
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
using ShortRoute.Client.Models.Tenants;
using ShortRoute.Client.Models.Generic;
using ShortRoute.Contracts.Commands.Authentication.Tenants;
using ShortRoute.Client.Models.Users;
using ShortRoute.Contracts.Queries.Common;

namespace ShortRoute.Client.Pages.Multitenancy;

public partial class Tenants
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    private ITenantsClient TenantsClient { get; set; } = default!;
    protected EntityServerTableContext<TenantModel, string, CreateUpdateTenantModel, CreateTenantCommand, UpdateTenantCommand> Context { get; set; } = default!;
    private List<TenantModel> _tenants = new();

    private bool _hasReadUsersPermission;

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthState).User;
        _hasReadUsersPermission = await AuthService.HasPermissionAsync(user, Permissions.ReadUsers);

        Context = new(
            entityName: L["Tenant"],
            entityNamePlural: L["Tenants"],
            searchPermission: Permissions.ReadTenants,
            createPermission: Permissions.CreateTenants,
            updatePermission: Permissions.UpdateTenants,
            deletePermission: Permissions.DeleteTenants,
            fields: new()
            {
                new(tenant => tenant.Dto.Id, L["Id"], "TenantId"),
                new(tenant => tenant.Dto.TenantFullName, L["Name"], "TenantFullName"),
                new(tenant => tenant.Dto.IsActive, L["Active"], Type: typeof(bool)),
            },
            idFunc: u => u.Dto.Id,
            searchFunc: async filter =>
            {
                var response = await TenantsClient.TenantsGetList(
                    filter?.Pagination?.ToString(),
                    filter?.Filter?.Text,
                    filter?.Sort?.ToString());

                return new PaginationResponse<TenantModel>
                {
                    Data = response.Tenants.Select(t => new TenantModel(t)).ToList(),
                    TotalCount = 0,
                };
            },
            createFunc: async command => await TenantsClient.TenantsCreate(command),
            updateFunc: async (_, command) => await TenantsClient.TenantsUpdate(command),
            deleteFunc: async id => await TenantsClient.TenantsDelete(id),

            hasExtraActionsFunc: () => _hasReadUsersPermission);
    }

    //private async Task ViewUpgradeSubscriptionModalAsync(string id)
    //{
    //    var tenant = _tenants.First(f => f.Dto.Id == id);
    //    var parameters = new DialogParameters
    //    {
    //        {
    //            nameof(UpgradeSubscriptionModal.Request),
    //            null,
    //            new UpgradeSubscriptionRequest
    //            {
    //                TenantId = tenant.Id,
    //                ExtendedExpiryDate = tenant.ValidUpto
    //            }
    //        }
    //    };
    //    var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
    //    var dialog = DialogService.Show<UpgradeSubscriptionModal>(L["Upgrade Subscription"], parameters, options);
    //    var result = await dialog.Result;
    //    if (!result.Cancelled)
    //    {
    //        await EntityTable.ReloadDataAsync();
    //    }
    //}

    private async Task ChangeTenantActivation(string id, bool isActive, Action onSuccess)
    {
        if (await ApiHelper.ExecuteClientCall(() => TenantsClient.TenantsChangeActive(new ChangeTenantActiveCommand
        {
            TenantId = id,
            IsActive = !isActive
        }), Snackbar))
        {
            onSuccess.Invoke();
        }
    }

    private async Task<IEnumerable<RoleDto>> GetTenantRoles()
    {
        if (await ApiHelper.ExecuteClientCall(() => TenantsClient.TenantsRoles(), Snackbar)
                is { } response)
        {
            return response.Roles;
        }

        return Enumerable.Empty<RoleDto>();
    }

    private void NavigateToUsers(string tenantId)
    {
        Navigation.NavigateTo($"users/{tenantId}");
    }
}