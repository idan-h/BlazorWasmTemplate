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
using ShortRoute.Client.Models;
using ShortRoute.Contracts.Auth;

namespace ShortRoute.Client.Pages.Multitenancy;

public partial class Tenants
{
    [Inject]
    private ITenantClient TenantsClient { get; set; } = default!;
    private string? _searchString;
    protected EntityClientTableContext<TenantModel, int, TenantDto> Context { get; set; } = default!;
    private List<TenantModel> _tenants = new();
    public EntityTable<TenantModel, int, TenantDto> EntityTable { get; set; } = default!;
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canUpgrade;
    private bool _canModify;

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: L["Tenant"],
            entityNamePlural: L["Tenants"],
            searchPermission: Permissions.TenantList,
            deletePermission: string.Empty,
            updatePermission: string.Empty,
            fields: new()
            {
                new(tenant => tenant.Dto.TenantId, L["Id"]),
                new(tenant => tenant.Dto.TenantFullName, L["Name"]),
                //new(tenant => tenant.Dto., L["Admin Email"]),
                //new(tenant => tenant.ValidUpto.ToString("MMM dd, yyyy"), L["Valid Upto"]),
                //new(tenant => tenant.IsActive, L["Active"], Type: typeof(bool))
            },
            loadDataFunc: async () => _tenants = (await TenantsClient.TenantsGetList()).Content!.Select(t => new TenantModel(t, false)).ToList(),
            searchFunc: (searchString, tenantDto) =>
                string.IsNullOrWhiteSpace(searchString)
                    || tenantDto.Dto.TenantFullName!.Contains(searchString, StringComparison.OrdinalIgnoreCase),
            createFunc: tenant => TenantsClient.TenantsCreate(tenant),
            hasExtraActionsFunc: () => true);

        var state = await AuthState;
        //_canUpgrade = await AuthService.HasPermissionAsync(state.User, FSHAction.UpgradeSubscription, FSHResource.Tenants);
        _canModify = await AuthService.HasPermissionAsync(state.User, Permissions.TenantUpdate);
    }

    private async Task ViewUpgradeSubscriptionModalAsync(int id)
    {
        var tenant = _tenants.First(f => f.Dto.TenantId == id);
        var parameters = new DialogParameters
        {
            {
                "",//nameof(UpgradeSubscriptionModal.Request),
                null
                //new UpgradeSubscriptionRequest
                //{
                //    TenantId = tenant.Id,
                //    ExtendedExpiryDate = tenant.ValidUpto
                //}
            }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<UpgradeSubscriptionModal>(L["Upgrade Subscription"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    private async Task DeactivateTenantAsync(string id)
    {
        //if (await ApiHelper.ExecuteClientCall(
        //    () => TenantsClient.DeactivateAsync(id),
        //    Snackbar,
        //    null,
        //    L["Tenant Deactivated."]) is not null)
        //{
        //    await EntityTable.ReloadDataAsync();
        //}
    }

    private async Task ActivateTenantAsync(string id)
    {
        //if (await ApiHelper.ExecuteClientCall(
        //    () => TenantsClient.ActivateAsync(id),
        //    Snackbar,
        //    null,
        //    L["Tenant Activated."]) is not null)
        //{
        //    await EntityTable.ReloadDataAsync();
        //}
    }
}