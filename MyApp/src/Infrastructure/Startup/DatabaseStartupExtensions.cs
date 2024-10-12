using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Infrastructure.Database;
using MyApp.Infrastructure.Database.EFCore;
using MyApp.Utilities.Settings;

namespace MyApp.Infrastructure.Startup;

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
            }, ServiceLifetime.Singleton)
            .AddScoped<IAppDbContextFactory, AppDbContextFactory>()
            .AddTransient(sp => sp.GetRequiredService<IAppDbContextFactory>().CreateTransientDbContext())
            .AddScoped(sp => sp.GetRequiredService<IAppDbContextFactory>().CreateScopedDbContext())
            .AddSingleton<IDatabasePinger, DatabasePinger>()
            .AddSingleton<IDatabaseSeeder, DatabaseSeeder>();
    }
}
