using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Domain.UserManagement.EmailChangeConfirmation.Failures;

namespace MyApp.Application.Handlers.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public class ConfirmEmailChangeCommandHandler(
    IUserContextAccessor userContextAccessor,
    IScopedDbContext dbContext
    ) : IRequestHandler<ConfirmEmailChangeRequest>
{
    public async Task Handle(ConfirmEmailChangeRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, oldEmail, _) = userContextAccessor.AccessToken;
        var (oldEmailCode, newEmailCode) = request;

        var user = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Include(u => u.EmailChangeConfirmation)
            .FindUser(userId, cancellationToken);

        if (user.EmailChangeConfirmation == null)
            throw EmailChangeNotRequestedFailure.Exception();

        if (user.EmailChangeConfirmation.CreatedAt < DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes))
            throw EmailChangeExpiredFailure.Exception();

        if (oldEmailCode != user.EmailChangeConfirmation.OldEmailCode || newEmailCode != user.EmailChangeConfirmation.NewEmailCode)
            throw EmailChangeInvalidCodesFailure.Exception();

        var emailAlreadyInUse = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == user.EmailChangeConfirmation.NewEmail, cancellationToken);

        if (emailAlreadyInUse)
            throw UserConflictFailure.EmailException();

        user.ConfirmEmailChange();
        dbContext.Remove(user.EmailChangeConfirmation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
