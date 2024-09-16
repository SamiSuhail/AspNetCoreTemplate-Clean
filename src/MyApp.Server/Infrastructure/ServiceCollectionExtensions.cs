using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Infrastructure.Email;
using MyApp.Server.Infrastructure.Logging;
using MyApp.Server.Infrastructure.Messaging;

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
            .AddCustomLogging(configuration);

        return services;
    }
}
