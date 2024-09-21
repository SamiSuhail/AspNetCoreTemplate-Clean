using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Database;

namespace MyApp.Application.Commands.Auth.Registration.Register;

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

        UserConflictFailure.Create(usernameTaken, emailTaken)
            .ThrowOnError();

        var user = UserEntity.Create(request.Username, request.Password, request.Email);
        dbContext.Add(user);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new SendUserConfirmationMessage(user.Username, user.Email, user.UserConfirmation!.Code), cancellationToken);
        }, cancellationToken);
    }
}
