using MediatR;
using MyApp.Server.Shared;
using Quartz;

namespace MyApp.Server.Application.BackgroundJobs.CleanupConfirmations;

public class CleanupConfirmationsBackgroundJob(ISender sender) : BaseBackgroundJob<CleanupConfirmationsBackgroundJob>, IJob
{
    public Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupConfirmationsRequest(), context.CancellationToken);
}
