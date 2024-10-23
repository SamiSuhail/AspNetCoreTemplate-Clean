using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Infra.Instance;

namespace MyApp.Application.Infrastructure.BackgroundJobs.Cleanup.Instances;

public class CleanupInstancesRequest() : IRequest;

public class CleanupInstancesHandler(IAppDbContextFactory dbContextFactory) : IRequestHandler<CleanupInstancesRequest>
{
    public async Task Handle(CleanupInstancesRequest request, CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateTransientDbContextAsync(cancellationToken);
        var expirationTime = DateTime.UtcNow.AddHours(-InstanceConstants.ExpirationTimeHours);
        await dbContext.Set<InstanceEntity>()
            .Where(uc => uc.CreatedAt < expirationTime && uc.IsCleanupEnabled)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
