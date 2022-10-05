using Riok.Mapperly.Abstractions;
using ShortRoute.Client.Models.Generic;
using ShortRoute.Client.Models.Users;
using ShortRoute.Contracts.Commands.Authentication.Users;
using ShortRoute.Contracts.Dtos.Authentication;

namespace ShortRoute.Client.Mappings.Users;

[Mapper]
public static partial class NoneMapper
{
    public static partial None Map(this None none);
}
