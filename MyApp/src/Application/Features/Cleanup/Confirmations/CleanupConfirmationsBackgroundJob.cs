using MediatR;
using MyApp.Application.Infrastructure;
using Quartz;

namespace MyApp.Application.Features.Cleanup.Confirmations;

public class CleanupConfirmationsBackgroundJob(ISender sender) : BaseBackgroundJob<CleanupConfirmationsBackgroundJob>
{
    public override Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupConfirmationsRequest(), context.CancellationToken);
}
