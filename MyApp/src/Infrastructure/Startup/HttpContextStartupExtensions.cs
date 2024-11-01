using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Infrastructure.Http;

namespace MyApp.Infrastructure.Startup;

public static class HttpContextStartupExtensions
{
    public static IServiceCollection AddCustomHttpContextAccessors(this IServiceCollection services)
    {
        services.AddHttpContextAccessor()
            .AddSingleton<IUserContextAccessor, HttpUserContextAccessor>();

        return services;
    }
}
