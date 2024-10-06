using MyApp.Infrastructure.BackgroundJobs;
using MyApp.Infrastructure.Database;
using MyApp.Utilities.Settings;
using Quartz;

namespace MyApp.Infrastructure.Startup;

public static class BackgroundJobsStartupExtensions
{
    public static IServiceCollection AddCustomBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
        => AddCustomBackgroundJobs(services, configuration, BackgroundScheduleStarter.StartAll);

    public static IServiceCollection AddCustomBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IServiceCollectionQuartzConfigurator, BackgroundJobsSettings> addJobsAction)
    {
        var backgroundJobsSettings = services.AddCustomSettings<BackgroundJobsSettings>(configuration);
        if (!backgroundJobsSettings.Enabled)
            return services;

        var connectionStrings = ConnectionStringsSettings.Get(configuration);
        services.AddQuartz(c =>
        {
            addJobsAction.Invoke(c, backgroundJobsSettings);

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