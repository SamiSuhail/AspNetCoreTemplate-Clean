using MyApp.Application.Startup;

namespace MyApp.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddCustomMediatR()
            .AddCustomRequestTransformers()
            .AddCustomValidators();

        return services;
    }
}
