namespace ShortRoute.Client.Infrastructure.Auth.Extensions;

public static class ClaimsPrincipalExtensions
{
    private static class AppClaimTypes
    {
        public const string Id = "nameid";
        public const string Name = "unique_name";
        public const string Email = "email";
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(AppClaimTypes.Email/*ClaimTypes.Email*/);

    public static string? GetFullName(this ClaimsPrincipal principal)
        => principal?.FindFirst(AppClaimTypes.Name/*ClaimTypes.Name*/)?.Value;

    public static string? GetUserId(this ClaimsPrincipal principal)
       => principal.FindFirstValue(AppClaimTypes.Id/*ClaimTypes.NameIdentifier*/);

    public static string? GetImageUrl(this ClaimsPrincipal principal)
       => principal.FindFirstValue(ClaimTypes.UserData);

    private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
        principal is null
            ? throw new ArgumentNullException(nameof(principal))
            : principal.FindFirst(claimType)?.Value;
}
