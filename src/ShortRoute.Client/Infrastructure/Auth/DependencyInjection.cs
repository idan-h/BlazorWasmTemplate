using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;
using ShortRoute.Client.Infrastructure.Auth.AzureAd;
using ShortRoute.Client.Infrastructure.Auth.Enums;
using ShortRoute.Client.Infrastructure.Auth.Handlers;
using ShortRoute.Client.Infrastructure.Auth.Jwt;

namespace ShortRoute.Client.Infrastructure.Auth;

internal static class DependencyInjection
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services = config[nameof(AuthProvider)] switch
        {
            // AzureAd
            nameof(AuthProvider.AzureAd) => services
                .AddScoped<IAuthenticationService, AzureAdAuthenticationService>()
                .AddScoped<AzureAdAuthorizationMessageHandler>()
                .AddMsalAuthentication(options =>
                {
                    config.Bind(nameof(AuthProvider.AzureAd), options.ProviderOptions.Authentication);
                    options.ProviderOptions.DefaultAccessTokenScopes.Add(
                        config[$"{nameof(AuthProvider.AzureAd)}:{ConfigNames.ApiScope}"]);
                    options.ProviderOptions.LoginMode = "redirect";
                })
                    .AddAccountClaimsPrincipalFactory<AzureAdClaimsPrincipalFactory>()
                    .Services,

            // Jwt
            _ => services
                .AddScoped<AuthenticationStateProvider, JwtAuthenticationService>()
                .AddScoped(sp => (IAuthenticationService)sp.GetRequiredService<AuthenticationStateProvider>())
                .AddScoped(sp => (IAccessTokenProvider)sp.GetRequiredService<AuthenticationStateProvider>())
                .AddScoped<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>()
                .AddScoped<JwtAuthenticationHeaderHandler>()
        };

        return services.AddScoped<CookieHandler>();
    }

    public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder, IConfiguration config)
    {
        builder = config[nameof(AuthProvider)] switch
        {
            // AzureAd
            nameof(AuthProvider.AzureAd) =>
                builder.AddHttpMessageHandler<AzureAdAuthorizationMessageHandler>(),

            // Jwt
            _ => builder.AddHttpMessageHandler<JwtAuthenticationHeaderHandler>()
        };

        return builder.AddHttpMessageHandler<CookieHandler>();
    }
}