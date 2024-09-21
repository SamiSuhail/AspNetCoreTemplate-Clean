using MyApp.Server.Application.Commands.BackgroundJobs.CleanupConfirmations;
using MyApp.Server.Infrastructure.Utilities;
using MyApp.Server.Shared;

namespace MyApp.Server.Infrastructure.BackgroundJobs;

public class BackgroundJobsSettings : BaseSettings<BackgroundJobsSettings>
{
    public required bool Enabled { get; set; }

    public BackgroundJobSettings<CleanupConfirmationsBackgroundJob>? CleanupConfirmations { get; set; }
}

public class BackgroundJobSettings<TBackgroundJob>  where TBackgroundJob : BaseBackgroundJob<TBackgroundJob>
{
    public required bool Enabled { get; set; }
    public string? CronSchedule { get; set; }
}