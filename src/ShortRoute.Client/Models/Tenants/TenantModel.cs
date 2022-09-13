using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Models.Tenants;

public class TenantModel
{
    public TenantDto Dto { get; }
    public bool ShowDetails { get; set; }

    public TenantModel(TenantDto dto, bool showDetails)
    {
        Dto = dto;
        ShowDetails = showDetails;
    }
}
