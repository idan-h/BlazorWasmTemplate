namespace ShortRoute.Client.Helpers;

public class DebugHelper
{
    public static void LogAllServices(IServiceCollection services, IServiceProvider provider)
    {
        var logger = provider.GetRequiredService<ILogger<DebugHelper>>();
        foreach (var svc in services)
        {
            logger.LogInformation("{name}, {lifetime}, {type}", svc.ServiceType.FullName, svc.Lifetime, svc.ImplementationType?.FullName);
        }
    }
}
