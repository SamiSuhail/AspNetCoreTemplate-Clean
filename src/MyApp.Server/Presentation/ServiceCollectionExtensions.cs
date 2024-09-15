using MyApp.Server.Presentation.Startup;
using MyApp.Server.Presentation.Startup.Middleware;

namespace MyApp.Server.Presentation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddCustomGraphQL(env)
            .AddCustomSwagger()
            .AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }
}
