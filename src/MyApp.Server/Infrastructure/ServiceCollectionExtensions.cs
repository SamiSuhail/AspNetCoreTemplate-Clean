using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Startup;
using MyApp.Server.Infrastructure.Utilities;

namespace MyApp.Server.Infrastructure;

public static class ServiceCollectionExtensions
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
}
