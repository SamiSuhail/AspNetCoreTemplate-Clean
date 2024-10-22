using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Infrastructure.Auth;
using MyApp.Infrastructure.Logging;

namespace MyApp.Infrastructure.Startup;

public static class LoggingStartupExtensions
{
    public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddSerilog((sp, lc) =>
        {
            lc.ReadFrom.Configuration(configuration);
            lc.Enrich.With(new UserDataLogEventEnricher(sp.GetRequiredService<IUserContextAccessor>()));
        });
        services.AddTransient(typeof(ILogger<>), typeof(Logger<>));

        return services;
    }

    public static WebApplication UseCustomSerilog(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (httpContext, _, _) => httpContext.Response.StatusCode switch
            {
                < 500 => LogEventLevel.Information,
                _ => LogEventLevel.Error,
            };
        });
        return app;
    }
}
