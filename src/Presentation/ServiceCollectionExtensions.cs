using MyApp.Presentation.Startup;
using MyApp.Presentation.Startup.Middleware;

namespace MyApp.Presentation;

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
