using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Commands.Auth.Registration.Register;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Domain.Infra.Instance;
using MyApp.Domain.Infra.Instance.Failures;

namespace MyApp.Application.Commands.Auth.Registration.Register;

public class RegisterCommandHandler(
    IScopedDbContext dbContext,
    IUnauthorizedRequestContextAccessor requestContextAccessor,
    IMessageProducer messageProducer) : IRequestHandler<RegisterRequest>
{
    public async Task Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var users = dbContext.Set<UserEntity>()
            .IgnoreQueryFilters();

        var usernameTaken = await users.AnyAsync(u => u.Username == request.Username, cancellationToken);
        var emailTaken = await users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        UserConflictFailure.Create(usernameTaken, emailTaken)
            .ThrowOnError();

        var instance = await dbContext.Set<InstanceEntity>()
            .Where(i => i.Name == requestContextAccessor.InstanceName)
            .Select(i => new
            {
                i.Id,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw InstanceNotFoundFailure.Exception();

        var user = UserEntity.Create(instance.Id, request.Username, request.Password, request.Email);
        dbContext.Add(user);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new SendUserConfirmationMessage(user.Username, user.Email, user.UserConfirmation!.Code), cancellationToken);
        }, cancellationToken);
    }
}
