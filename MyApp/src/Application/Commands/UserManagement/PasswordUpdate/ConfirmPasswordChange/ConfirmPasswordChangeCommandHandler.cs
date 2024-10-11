using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Application.Utilities;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation.Failures;

namespace MyApp.Application.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public class ConfirmPasswordChangeCommandHandler(IRequestContextAccessor userContextAccessor, IScopedDbContext dbContext) : IRequestHandler<ConfirmPasswordChangeRequest>
{
    public async Task Handle(ConfirmPasswordChangeRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, _) = userContextAccessor.AccessToken;

        var user = await dbContext.Set<UserEntity>()
            .Include(u => u.PasswordChangeConfirmation)
            .FindUser(userId, cancellationToken);

        if (user.PasswordChangeConfirmation == null)
            throw PasswordChangeNotRequestedFailure.Exception();

        if (request.Code != user.PasswordChangeConfirmation.Code)
            throw PasswordChangeInvalidCodeFailure.Exception();

        var expirationTime = DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);
        if (user.PasswordChangeConfirmation.CreatedAt < expirationTime)
            throw PasswordChangeExpiredFailure.Exception();

        user.ConfirmPasswordChange(user.PasswordChangeConfirmation.NewPasswordHash);
        dbContext.Remove(user.PasswordChangeConfirmation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
