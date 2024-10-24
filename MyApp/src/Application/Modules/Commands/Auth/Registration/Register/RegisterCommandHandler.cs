using MediatR;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Infrastructure.Repositories.Instance;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.Register;
using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Application.Modules.Commands.Auth.Registration.Register;

public record RegisterCommand(RegisterRequest Request, string? InstanceName) : IRequest;

public class RegisterCommandHandler(
    IScopedDbContext dbContext,
    IMessageProducer messageProducer
    ) : IRequestHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var (request, instanceName) = command;
        var instanceId = await dbContext.GetInstanceId(instanceName, cancellationToken);

        var users = dbContext.Set<UserEntity>()
            .IgnoreQueryFilters();

        var usernameTaken = await users.AnyAsync(u => u.Username == request.Username && u.InstanceId == instanceId, cancellationToken);
        var emailTaken = await users.AnyAsync(u => u.Email == request.Email && u.InstanceId == instanceId, cancellationToken);

        UserConflictFailure.Create(usernameTaken, emailTaken)
            .ThrowOnError();

        var user = UserEntity.Create(instanceId, request.Username, request.Password, request.Email);
        dbContext.Add(user);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new SendRegisterUserConfirmationMessage(user.Username, user.Email, user.UserConfirmation!.Code), cancellationToken);
        }, cancellationToken);
    }
}
