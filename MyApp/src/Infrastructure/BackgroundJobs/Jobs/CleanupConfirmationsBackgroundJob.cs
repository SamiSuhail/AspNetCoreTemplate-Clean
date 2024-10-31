using MediatR;
using MyApp.Application.Modules.BackgroundJobs;
using MyApp.Application.Modules.BackgroundJobs.Cleanup.Confirmations;
using Quartz;

namespace MyApp.Infrastructure.BackgroundJobs.Jobs;

public class CleanupConfirmationsBackgroundJob(ISender sender) : BaseBackgroundJob<CleanupConfirmationsBackgroundJob>
{
    public override Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupConfirmationsRequest(), context.CancellationToken);
}
