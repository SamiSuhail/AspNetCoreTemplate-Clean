using MediatR;
using MyApp.Application.Infrastructure;
using MyApp.Application.Infrastructure.BackgroundJobs.Cleanup.Confirmations;
using Quartz;

namespace MyApp.Infrastructure.BackgroundJobs.Jobs;

public class CleanupConfirmationsBackgroundJob(ISender sender) : BaseBackgroundJob<CleanupConfirmationsBackgroundJob>
{
    public override Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupConfirmationsRequest(), context.CancellationToken);
}
