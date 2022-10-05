using Riok.Mapperly.Abstractions;
using ShortRoute.Client.Models.Users;
using ShortRoute.Contracts.Commands.Authentication.Roles;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Mappings.Roles;

[Mapper(UseDeepCloning = true)]
public static partial class RolesMapper
{
    public static partial CreateRoleCommand ToCreateCommand(this CreateUpdateRoleModel model);
    public static partial UpdateRoleCommand ToUpdateCommand(this CreateUpdateRoleModel model);

    public static partial CreateUpdateRoleModel ToModel(this RoleDto dto);
}
