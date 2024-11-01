using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Infra.Instance.Failures;
using MyApp.Domain.Infra.Instance;

namespace MyApp.Application.Infrastructure.Repositories.Instance;

public static class InstanceDbContextExtensions
{
    public static async Task<int> GetInstanceId(
        this IBaseDbContext dbContext,
        string? instanceName,
        CancellationToken cancellationToken)
    {
        var instanceNameToUse = instanceName ?? InstanceConstants.DefaultInstanceName;

        var instance = await dbContext.Set<InstanceEntity>()
            .Where(x => x.Name == instanceNameToUse)
            .Select(x => new { x.Id })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw InstanceNotFoundFailure.Exception();

        return instance.Id;
    }
}
