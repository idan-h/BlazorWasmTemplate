using ShortRoute.Client.Helpers;

namespace ShortRoute.Client.Mappings.Generic;

public static class MappingByReflectionHelper
{
    public static TOutput MapDynamically<TOutput, TInput>(this TInput input)
        where TOutput : new()
    {
        var method = ReflectionHelper.GetMethodsBySignature(typeof(TOutput), typeof(TInput))
            .FirstOrDefault(m => m.IsStatic && (m.Name.StartsWith("To") || m.DeclaringType!.Name.EndsWith("Mapper")));

        if (method is null)
        {
            throw new InvalidOperationException($"Can't find mapping method for type {typeof(TInput)} to {typeof(TOutput)}");
        }

        return (TOutput)method.Invoke(null, new object[] { input! })!;
    }
}