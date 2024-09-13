using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Infrastructure.Messaging;
using MyApp.Server.Modules.Commands.Auth.ConfirmRegistration;

namespace MyApp.Server.Modules.Commands.Auth.Register;

public record RegisterRequest(
    string Email,
    string Username,
    string Password) : IRequest;

public class RegisterCommandHandler(IScopedDbContext dbContext, IMessageProducer messageProducer) : IRequestHandler<RegisterRequest>
{
    public async Task Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var users = dbContext.Set<UserEntity>()
            .IgnoreQueryFilters();

        var usernameTaken = await users.AnyAsync(u => u.Username == request.Username, cancellationToken);
        var emailTaken = await users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        RegisterConflictFailure.Create(usernameTaken, emailTaken)
            .ThrowOnError();

        var emailConfirmation = EmailConfirmationEntity.Create();
        var user = UserEntity.Create(request.Username, request.Password, request.Email, emailConfirmation);
        dbContext.Add(user);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new ConfirmRegistrationMessage(user.Username, user.Email, emailConfirmation.Code), cancellationToken);
        }, cancellationToken);
    }
}
