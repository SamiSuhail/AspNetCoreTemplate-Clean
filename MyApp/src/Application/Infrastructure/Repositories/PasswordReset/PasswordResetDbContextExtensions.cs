using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Infrastructure.Repositories.User;
using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Infrastructure.Repositories.PasswordReset;

public static class PasswordResetDbContextExtensions
{
    public static async Task ConfirmPasswordReset(this IBaseDbContext dbContext, int userId, string code, string password, CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<UserEntity>()
                    .IgnoreQueryFilters()
                    .Include(u => u.PasswordResetConfirmation)
                    .FindUser(userId, cancellationToken);

        if (user.PasswordResetConfirmation == null)
            throw PasswordResetNotRequestedFailure.Exception();

        if (code != user.PasswordResetConfirmation.Code)
            throw PasswordResetInvalidCodeFailure.Exception();

        var expirationTime = DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);
        if (user.PasswordResetConfirmation.CreatedAt < expirationTime)
            throw PasswordResetExpiredFailure.Exception();

        user.UpdatePassword(password);
        user.SignOutOnAllDevices();
        dbContext.Remove(user.PasswordResetConfirmation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}