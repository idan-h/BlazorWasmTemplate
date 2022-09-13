using Riok.Mapperly.Abstractions;
using ShortRoute.Client.Models.Users;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Mappings.Users;

[Mapper]
public static partial class UsersMapper
{
    public static partial CreateUserCommand ToCreateCommand(this CreateUpdateUserModel model);
    public static partial UpdateUserCommand ToUpdateCommand(this CreateUpdateUserModel model);

    public static partial CreateUpdateUserModel ToModel(this UserDto dto);
}
