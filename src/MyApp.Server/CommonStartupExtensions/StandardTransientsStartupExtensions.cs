using System.Reflection;

namespace MyApp.Server.CommonStartupExtensions;

public static class StandardTransientsStartupExtensions
{
    public static IServiceCollection AddCustomStandardTransients(this IServiceCollection services)
    {
        var types = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(t => t.Name.EndsWith("EmailNotifier"));

        var interfaces = types.Where(t => t.IsInterface);
        var implementations = types.Where(t => !t.IsInterface);

        foreach (var typeInfo in implementations)
        {
            var matchingInterface = interfaces.First(t => t.Name == $"I{typeInfo.Name}");
            services.AddTransient(matchingInterface, typeInfo);
        }

        return services;
    }
}
