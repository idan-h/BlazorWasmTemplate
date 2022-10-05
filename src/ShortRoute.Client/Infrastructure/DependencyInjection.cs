using System.Globalization;
using ShortRoute.Client.Infrastructure.Auth;
using ShortRoute.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using ShortRoute.Client.Infrastructure.ApiClient;
using ShortRoute.Client.Infrastructure.ApiClient.v1;
using Refit;
using ShortRoute.Client.Infrastructure.ApiClient.Base;
using ShortRoute.Client.Infrastructure.Auth.Enums;
using ShortRoute.Contracts.Enums;

namespace ShortRoute.Client.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration config) =>
        services
        .AddLocalization(options => options.ResourcesPath = "Resources")
        .AddBlazoredLocalStorage()
        .AddMudServices(configuration =>
            {
                configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                configuration.SnackbarConfiguration.ShowCloseIcon = false;
            })
        .AddSingleton<IClientPreferenceManager, ClientPreferenceManager>()
        .AutoRegisterInterfaces<IAppService>()
        .AutoRegisterRefitClients<IApiClient>(config)
        .AddAuthentication(config)
        .AddAuthorizationCore(RegisterPermissionClaims);

        // Add Api Http Client.
        // .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName));

    private static void RegisterPermissionClaims(AuthorizationOptions options)
    {
        foreach (var permission in Permissions.All)
        {
            options.AddPolicy(permission, policy => policy.RequireClaim(AuthClaimTypes.Permission, permission));
        }
    }

    public static IServiceCollection AutoRegisterInterfaces<T>(this IServiceCollection services)
    {
        var @interface = typeof(T);

        var types = @interface
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterface($"I{t.Name}"),
                Implementation = t
            })
            .Where(t => t.Service != null);

        foreach (var type in types)
        {
            if (@interface.IsAssignableFrom(type.Service))
            {
                services.AddTransient(type.Service, type.Implementation);
            }
        }

        return services;
    }

    public static IServiceCollection AutoRegisterRefitClients<T>(this IServiceCollection services, IConfiguration config)
    {
        var inter = typeof(T);

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsInterface && inter.IsAssignableFrom(p) && p != inter);

        foreach (var type in types)
        {
            services
                .AddRefitClient(type)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(config[ConfigNames.ApiBaseUrl]))
                .AddAuthenticationHandler(config);
        }

        return services;
    }
}