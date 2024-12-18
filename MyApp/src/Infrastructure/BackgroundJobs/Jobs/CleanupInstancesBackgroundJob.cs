﻿using MediatR;
using MyApp.Application.Modules.BackgroundJobs;
using MyApp.Application.Modules.BackgroundJobs.Cleanup.Instances;
using Quartz;

namespace MyApp.Infrastructure.BackgroundJobs.Jobs;

public class CleanupInstancesBackgroundJob(ISender sender) : BaseBackgroundJob<CleanupInstancesBackgroundJob>
{
    public override Task Execute(IJobExecutionContext context)
        => sender.Send(new CleanupInstancesRequest(), context.CancellationToken);
}
