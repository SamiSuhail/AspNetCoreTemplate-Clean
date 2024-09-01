using Quartz;

namespace MyApp.Server.Modules.Commands.Auth.BackgroundJobs.CleanupConfirmations;

public static class CleanupConfirmationsScheduler
{
    const string CronSchedule = "0 8 * * * ?";
    public static void Start(IServiceCollectionQuartzConfigurator options)
    {
        var jobKey = JobKey.Create(nameof(CleanupConfirmationsBackgroundJob));

        options.AddJob<CleanupConfirmationsBackgroundJob>(jb =>
            jb.WithIdentity(jobKey)
                .DisallowConcurrentExecution()
                .Build());

        options.AddTrigger(tb =>
            tb.WithIdentity(nameof(CleanupConfirmationsBackgroundJob))
                .ForJob(jobKey)
                .WithCronSchedule(CronSchedule));
    }
}
