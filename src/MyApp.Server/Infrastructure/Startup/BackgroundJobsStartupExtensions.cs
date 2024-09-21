using MyApp.Server.Infrastructure.BackgroundJobs;
using MyApp.Server.Infrastructure.Database;
using Quartz;

namespace MyApp.Server.Infrastructure.Startup;

public static class BackgroundJobsStartupExtensions
{
    public static IServiceCollection AddCustomBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var backgroundJobsSettings = services.AddCustomSettings<BackgroundJobsSettings>(configuration);
        if (!backgroundJobsSettings.Enabled)
            return services;

        var connectionStrings = ConnectionStringsSettings.Get(configuration);
        services.AddQuartz(c =>
        {
            BackgroundScheduleStarter.StartAll(c, backgroundJobsSettings);

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