using MyApp.Server.Application.Commands.Auth.BackgroundJobs.CleanupConfirmations;
using MyApp.Server.Infrastructure.Database;
using Quartz;

namespace MyApp.Server.Application.Startup;

public static class BackgroundJobsStartupExtensions
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
