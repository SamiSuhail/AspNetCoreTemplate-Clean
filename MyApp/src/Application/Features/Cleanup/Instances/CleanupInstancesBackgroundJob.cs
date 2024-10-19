using MediatR;
using MyApp.Application.Infrastructure;
using Quartz;

namespace MyApp.Application.Features.Cleanup.Instances;

public class CleanupInstancesBackgroundJob(ISender sender) : BaseBackgroundJob<CleanupInstancesBackgroundJob>
{
    public override Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupInstancesRequest(), context.CancellationToken);
}
