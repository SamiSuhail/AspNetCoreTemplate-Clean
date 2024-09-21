using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Infrastructure.Auth;
using MyApp.Infrastructure.Startup;
using MyApp.Infrastructure.Utilities;

namespace MyApp.Infrastructure;

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
