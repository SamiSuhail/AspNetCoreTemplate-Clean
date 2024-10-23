using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Presentation.Interfaces.Http.Commands.Infra.CreateInstance;
using MyApp.Domain.Infra.Instance;
using MyApp.Domain.Infra.Instance.Failures;

namespace MyApp.Application.Modules.Commands.Infra.CreateInstance;

public class CreateInstanceCommandHandler(
    IScopedDbContext dbContext
    ) : IRequestHandler<CreateInstanceRequest>
{
    public async Task Handle(CreateInstanceRequest request, CancellationToken cancellationToken)
    {
        var (name, isCleanupEnabled) = request;

        var nameIsTaken = await dbContext.Set<InstanceEntity>()
            .Where(i => i.Name == name)
            .AnyAsync(cancellationToken);

        if (nameIsTaken)
            throw InstanceNameTakenFailure.Exception();

        var instance = InstanceEntity.Create(name, isCleanupEnabled);
        dbContext.Add(instance);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
