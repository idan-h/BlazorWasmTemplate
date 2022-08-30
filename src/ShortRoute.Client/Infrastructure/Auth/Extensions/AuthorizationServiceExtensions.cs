using Microsoft.AspNetCore.Authorization;

namespace ShortRoute.Client.Infrastructure.Auth.Extensions;

public static class AuthorizationServiceExtensions
{
    public static async Task<bool> HasPermissionAsync(this IAuthorizationService service, ClaimsPrincipal user, string permission) =>
        (await service.AuthorizeAsync(user, null, permission)).Succeeded;
}