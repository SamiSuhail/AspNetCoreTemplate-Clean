using MediatR;
using Quartz;

namespace MyApp.Server.Modules.Commands.Auth.BackgroundJobs.CleanupConfirmations;

public class CleanupConfirmationsBackgroundJob(ISender sender) : IJob
{
    public Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupConfirmationsRequest(), context.CancellationToken);
}
