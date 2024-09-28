using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.EmailChangeConfirmation;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.UserManagement.EmailChangeConfirmation.Failures;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;

namespace MyApp.Application.Commands.UserManagement.EmailUpdate.ChangeEmail;

public record ChangeEmailRequest(string Email) : IRequest;

public class ChangeEmailCommandHandler(
    IUserContextAccessor userContextAccessor,
    IScopedDbContext dbContext,
    IMessageProducer messageProducer
    ) : IRequestHandler<ChangeEmailRequest>
{
    public async Task Handle(ChangeEmailRequest request, CancellationToken cancellationToken)
    {
        var (userId, username, oldEmail) = userContextAccessor.User;
        var newEmail = request.Email;

        if (oldEmail == newEmail)
            throw EmailIsIdenticalFailure.Exception();

        var emailAlreadyInUse = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == newEmail, cancellationToken);

        if (emailAlreadyInUse)
            throw UserConflictFailure.EmailException();

        var emailChangeConfirmation = EmailChangeConfirmationEntity.Create(userId, newEmail);
        dbContext.Add(emailChangeConfirmation);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.Set<EmailChangeConfirmationEntity>()
                .Where(ecc => ecc.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(
                new ChangeEmailMessage(
                    username,
                    oldEmail,
                    newEmail,
                    emailChangeConfirmation.OldEmailCode,
                    emailChangeConfirmation.NewEmailCode),
                cancellationToken);
        }, cancellationToken);
    }
}
