using MyApp.Application.Features.Cleanup.Confirmations;
using MyApp.Application.Features.Cleanup.Instances;
using MyApp.Application.Infrastructure;
using MyApp.Utilities.Settings;

namespace MyApp.Infrastructure.BackgroundJobs;

public class BackgroundJobsSettings : BaseSettings<BackgroundJobsSettings>
{
    public required bool Enabled { get; set; }

    public BackgroundJobSettings<CleanupConfirmationsBackgroundJob>? CleanupConfirmations { get; set; }
    public BackgroundJobSettings<CleanupInstancesBackgroundJob>? CleanupInstances { get; set; }
}

public class BackgroundJobSettings<TBackgroundJob> : BackgroundJobSettings
    where TBackgroundJob : BaseBackgroundJob<TBackgroundJob>
{
}

public class BackgroundJobSettings
{
    public required bool Enabled { get; set; }
    public string? CronSchedule { get; set; }
}