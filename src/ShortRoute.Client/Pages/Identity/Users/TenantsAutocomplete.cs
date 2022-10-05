using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using ShortRoute.Client.Shared;
using ShortRoute.Contracts.Dtos.Authentication;
using ShortRoute.Contracts.Queries.Authentication.Tenants;
using ShortRoute.Contracts.Queries.Common;

namespace ShortRoute.Client.Pages.Identity.Users;

public class TenantsAutocomplete : MudAutocomplete<string>
{
    [Inject]
    private IStringLocalizer<SharedResource> L { get; set; } = default!;
    [Inject]
    private ITenantsClient TenantsClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<TenantDto> _tenants = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Tenant"];
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = Search;
        ToStringFunc = GetName;
        AdornmentIcon = Icons.Material.Filled.Search;
        AdornmentColor = Color.Primary;
        return base.SetParametersAsync(parameters);
    }

    // when the value parameter is set, we have to load that one brand to be able to show the name
    // we can't do that in OnInitialized because of a strange bug (https://github.com/MudBlazor/MudBlazor/issues/3818)
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender &&
            _value != default &&
            await ApiHelper.ExecuteClientCall(() => TenantsClient.TenantsGetSingle(_value), Snackbar) is { } tenant)
        {
            _tenants.Add(tenant);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<string>> Search(string value)
    {
        if (value.Length < 3)
        {
            _tenants = new();
            return Enumerable.Empty<string>();
        }

        if (await ApiHelper.ExecuteClientCall(() => TenantsClient.TenantsGetList(filter: value), Snackbar)
                is { } response)
        {
            _tenants = response.Tenants.ToList();
        }

        return _tenants.Select(x => x.Id);
    }

    private string GetName(string id) => _tenants.Find(b => b.Id == id)?.TenantFullName ?? "";
}

