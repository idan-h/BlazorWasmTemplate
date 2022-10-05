using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Models.Tenants;

public class TenantModel
{
    public TenantDto Dto { get; }

    public TenantModel(TenantDto dto)
    {
        Dto = dto;
    }
}
