using Microsoft.EntityFrameworkCore;

namespace MyApp.Server.Infrastructure.Database;

public static class DatabaseStartupExtensions
{
    public static IServiceCollection AddCustomDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStrings = ConnectionStringsSettings.Get(configuration);
        var dbSettings = DatabaseSettings.Get(configuration);

        services.AddSingleton(connectionStrings);
        services.AddSingleton(dbSettings);

        return services.AddDbContextFactory<AppDbContext>((sp, options) =>
            {
                var connectionString = sp.GetRequiredService<ConnectionStringsSettings>().Database;
                var settings = sp.GetRequiredService<DatabaseSettings>();
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(settings.MaxRetryCount);
                    npgsqlOptions.CommandTimeout(settings.CommandTimeout);
                });

                options.EnableDetailedErrors(settings.EnableDetailedErrors);
                options.EnableSensitiveDataLogging(settings.EnableSensitiveDataLogging);
            }, ServiceLifetime.Scoped)
            .AddScoped<IAppDbContextFactory, AppDbContextFactory>()
            .AddTransient(sp => sp.GetRequiredService<IAppDbContextFactory>().CreateTransientDbContext())
            .AddScoped(sp => sp.GetRequiredService<IAppDbContextFactory>().CreateScopedDbContext());
    }
}
