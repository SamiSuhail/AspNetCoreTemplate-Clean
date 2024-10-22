using MediatR;
using MyApp.Application.Features.Cleanup.Instances;
using MyApp.Application.Infrastructure;
using Quartz;

namespace MyApp.Infrastructure.BackgroundJobs.Jobs;

public class CleanupInstancesBackgroundJob(ISender sender) : BaseBackgroundJob<CleanupInstancesBackgroundJob>
{
    public override Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupInstancesRequest(), context.CancellationToken);
}
