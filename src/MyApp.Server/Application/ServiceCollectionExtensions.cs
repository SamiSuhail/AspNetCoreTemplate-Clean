using MyApp.Server.Application.Startup;

namespace MyApp.Server.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomBackgroundJobs(configuration)
            .AddCustomMediatR()
            .AddCustomRequestTransformers()
            .AddCustomValidators();

        return services;
    }
}
