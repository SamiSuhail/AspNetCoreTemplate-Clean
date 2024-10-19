using MyApp.Application.Infrastructure;
using Quartz;

namespace MyApp.Infrastructure.BackgroundJobs;

public static class BackgroundScheduleStarter
{
    public static void StartAll(IServiceCollectionQuartzConfigurator options, BackgroundJobsSettings settings)
    {
        options.StartSchedule(settings.CleanupConfirmations!);
        options.StartSchedule(settings.CleanupInstances!);
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

        options.AddJob<TBackgroundJob>(jb =>
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