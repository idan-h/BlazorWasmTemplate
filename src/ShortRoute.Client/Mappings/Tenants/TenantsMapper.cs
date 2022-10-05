using Riok.Mapperly.Abstractions;
using ShortRoute.Client.Models.Tenants;
using ShortRoute.Client.Models.Users;
using ShortRoute.Contracts.Commands.Authentication.Tenants;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Mappings.Users;

[Mapper(UseDeepCloning = true)]
public static partial class TenantsMapper
{
    public static partial CreateTenantCommand ToCreateCommand(this CreateUpdateTenantModel model);
    public static partial UpdateTenantCommand ToUpdateCommand(this CreateUpdateTenantModel model);

    public static partial CreateUpdateTenantModel ToModel(this TenantDto dto);
    public static CreateUpdateTenantModel ToCreateUpdateModel(this TenantModel model)
    {
        return model.Dto.ToModel();
    }
}
