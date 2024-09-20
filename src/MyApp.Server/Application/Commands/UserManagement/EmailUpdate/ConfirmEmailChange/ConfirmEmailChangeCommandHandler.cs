using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Application.Utilities;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Domain.Shared.Confirmations;
using MyApp.Server.Domain.UserManagement.EmailChangeConfirmation.Failures;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Application.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public record ConfirmEmailChangeRequest(string OldEmailCode, string NewEmailCode) : IRequest;

public class ConfirmEmailChangeCommandHandler(
    IUserContextAccessor userContextAccessor,
    IScopedDbContext dbContext
    ) : IRequestHandler<ConfirmEmailChangeRequest>
{
    public async Task Handle(ConfirmEmailChangeRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, oldEmail) = userContextAccessor.User;
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
