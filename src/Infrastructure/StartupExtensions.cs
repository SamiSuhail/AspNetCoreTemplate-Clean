using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Infrastructure.Auth;
using MyApp.Infrastructure.Startup;
using MyApp.Infrastructure.Utilities;

namespace Microsoft.Extensions.DependencyInjection;

public static class StartupExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor()
            .AddSingleton<IClock, Clock>()
            .AddCustomAuth(configuration)
            .AddCustomDatabase(configuration)
            .AddCustomEmail(configuration)
            .AddCustomMessaging(configuration)
            .AddCustomLogging(configuration)
            .AddCustomBackgroundJobs(configuration);

        return services;
    }

    public static WebApplication UseCustomInfrastructure(this WebApplication app)
    {
        app.UseCustomSerilog()
            .UseAuthentication()
            .UseAuthorization();

        return app;
    }
}
