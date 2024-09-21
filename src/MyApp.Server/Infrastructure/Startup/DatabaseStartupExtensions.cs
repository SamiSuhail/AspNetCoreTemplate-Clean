using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Infrastructure.Startup;

public static class DatabaseStartupExtensions
{
    public static IServiceCollection AddCustomDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomSettings<ConnectionStringsSettings>(configuration);
        services.AddCustomSettings<DatabaseSettings>(configuration);

        return services.AddDbContextFactory<AppDbContext>((sp, options) =>
            {
                var connectionString = sp.GetRequiredService<ConnectionStringsSettings>().Database;
                var dbSettings = sp.GetRequiredService<DatabaseSettings>();

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(dbSettings.MaxRetryCount);
                    npgsqlOptions.CommandTimeout(dbSettings.CommandTimeout);
                });

                options.EnableDetailedErrors(dbSettings.EnableDetailedErrors);
                options.EnableSensitiveDataLogging(dbSettings.EnableSensitiveDataLogging);
            }, ServiceLifetime.Scoped)
            .AddScoped<IAppDbContextFactory, AppDbContextFactory>()
            .AddTransient(sp => sp.GetRequiredService<IAppDbContextFactory>().CreateTransientDbContext())
            .AddScoped(sp => sp.GetRequiredService<IAppDbContextFactory>().CreateScopedDbContext())
            .AddSingleton<IDatabasePinger, DatabasePinger>();
    }
}
