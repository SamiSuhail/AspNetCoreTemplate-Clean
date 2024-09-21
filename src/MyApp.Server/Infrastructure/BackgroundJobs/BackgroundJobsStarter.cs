using MyApp.Application.BackgroundJobs.CleanupConfirmations;
using MyApp.Application.Utilities;
using Quartz;

namespace MyApp.Server.Infrastructure.BackgroundJobs;

public static class BackgroundScheduleStarter
{
    public static void StartAll(IServiceCollectionQuartzConfigurator options, BackgroundJobsSettings settings)
    {
        options.StartSchedule(settings.CleanupConfirmations!);
    }

    private static void StartSchedule<TBackgroundJob>(
        this IServiceCollectionQuartzConfigurator options,
        BackgroundJobSettings<TBackgroundJob> settings
        ) where TBackgroundJob : BaseBackgroundJob<TBackgroundJob>
    {
        if (!settings.Enabled)
            return;

        var name = BaseBackgroundJob<TBackgroundJob>.Name;
        var jobKey = JobKey.Create(name);

        options.AddJob<CleanupConfirmationsBackgroundJob>(jb =>
            jb.WithIdentity(jobKey)
                .DisallowConcurrentExecution()
                .Build());

        if (settings.CronSchedule != null)
        {
            options.AddTrigger(tb =>
                tb.WithIdentity(name)
                    .ForJob(jobKey)
                    .WithCronSchedule(settings.CronSchedule));
        }
    }
}