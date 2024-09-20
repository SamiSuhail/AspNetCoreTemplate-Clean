using MyApp.Server.Shared;
using Quartz;

namespace MyApp.Server.Application.Commands.BackgroundJobs.CleanupConfirmations;

public static class CleanupConfirmationsScheduler
{
    const string CronSchedule = "0 8 * * * ?";
    public static void Start(IServiceCollectionQuartzConfigurator options)
    {
        var name = TestsHelper.GetName(nameof(CleanupConfirmationsBackgroundJob));

        var jobKey = JobKey.Create(name);

        options.AddJob<CleanupConfirmationsBackgroundJob>(jb =>
            jb.WithIdentity(jobKey)
                .DisallowConcurrentExecution()
                .Build());

        options.AddTrigger(tb =>
            tb.WithIdentity(name)
                .ForJob(jobKey)
                .WithCronSchedule(CronSchedule));
    }
}
