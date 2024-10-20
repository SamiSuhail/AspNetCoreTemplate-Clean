using MyApp.Presentation.Startup;
using MyApp.Presentation.Startup.Middleware;

namespace Microsoft.Extensions.DependencyInjection;

public static class StartupExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddCustomGraphQL(env)
            .AddCustomSwagger()
            .AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    public static WebApplication UseCustomPresentation(this WebApplication app)
    {
        app.UseExceptionHandler(options => { });

        app.UseCustomSwagger()
            .MapCustomEndpoints()
            .MapCustomGraphQL();

        return app;
    }
}
