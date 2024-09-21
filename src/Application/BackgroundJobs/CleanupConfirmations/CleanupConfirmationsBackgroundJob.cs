using MediatR;
using MyApp.Application.Utilities;
using Quartz;

namespace MyApp.Application.BackgroundJobs.CleanupConfirmations;

public class CleanupConfirmationsBackgroundJob(ISender sender) : BaseBackgroundJob<CleanupConfirmationsBackgroundJob>, IJob
{
    public Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupConfirmationsRequest(), context.CancellationToken);
}
