using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Modules.Commands.Auth.BackgroundJobs.CleanupConfirmations;
using Quartz;

namespace MyApp.Server.Infrastructure.BackgroundJobs;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStrings = ConnectionStringsSettings.Get(configuration);

        services.AddQuartz(c =>
        {
            CleanupConfirmationsScheduler.Start(c);

            c.UsePersistentStore(o =>
            {
                o.UsePostgres(connectionStrings.Database);
                o.UseNewtonsoftJsonSerializer();
            });
        });

        services.AddQuartzHostedService(o =>
        {
            o.WaitForJobsToComplete = true;
        });

        return services;
    }
}
