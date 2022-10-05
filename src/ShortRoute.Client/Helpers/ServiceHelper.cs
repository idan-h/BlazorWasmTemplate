namespace ShortRoute.Client.Helpers;

public static class ServiceHelper
{
    public static IServiceProvider Provider { private get; set; } = default!;

    private static T _execScope<T>(Func<IServiceScope, T> func)
    {
        if (Provider == null)
        {
            throw new InvalidOperationException("Provider cannot be null - ServiceHelper");
        }

        using var scope = Provider.CreateScope();
        return func(scope);
    }

    public static T? GetService<T>()
    {
        return _execScope<T?>(s => s.ServiceProvider.GetService<T>());
    }

    public static T GetRequiredService<T>()
        where T : notnull
    {
        return _execScope<T>(s => s.ServiceProvider.GetRequiredService<T>());
    }
}
