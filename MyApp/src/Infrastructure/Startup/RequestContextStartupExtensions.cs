using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Infrastructure.RequestContext;

namespace MyApp.Infrastructure.Startup;

public static class RequestContextStartupExtensions
{
    public static IServiceCollection AddCustomRequestContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor()
            .AddScoped<IRequestContextAccessor, RequestContextAccessor>()
            .AddScoped<IUnauthorizedRequestContextAccessor, UnauthorizedRequestContextAccessor>();

        return services;
    }
}
