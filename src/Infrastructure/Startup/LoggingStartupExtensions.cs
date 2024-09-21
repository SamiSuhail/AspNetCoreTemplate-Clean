using Serilog.Events;
using Serilog;

namespace MyApp.Infrastructure.Startup;

public static class LoggingStartupExtensions
{
    public static void CreateCustomSerilogBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();
    }

    public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        services.AddSerilog((sp, lc) =>
        {
            lc.ReadFrom.Configuration(configuration);
        });

        return services;
    }

    public static WebApplication UseCustomSerilog(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (httpContext, _, _) => httpContext.Response.StatusCode switch
            {
                < 400 => LogEventLevel.Information,
                < 500 => LogEventLevel.Warning,
                _ => LogEventLevel.Error,
            };
        });
        return app;
    }
}
