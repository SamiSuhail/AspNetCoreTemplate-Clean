using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Application.Infrastructure.Repositories.PasswordReset;

public interface IPasswordResetRepository
{
    Task SendConfirmation(int userId, string username, string email, CancellationToken cancellationToken);
}

public class PasswordResetRepository(
    ITransientDbContext dbContext,
    IMessageProducer messageProducer
    ) : IPasswordResetRepository
{
    public async Task SendConfirmation(int userId, string username, string email, CancellationToken cancellationToken)
    {
        var userData = await dbContext.Set<UserEntity>()
                    .IgnoreQueryFilters()
                    .Where(u => u.Id == userId)
                    .Select(u => new
                    {
                        u.PasswordHash,
                        u.PasswordResetConfirmation,
                    })
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw UserNotFoundFailure.Exception();

        if (userData.PasswordResetConfirmation != null)
            dbContext.Remove(userData.PasswordResetConfirmation);

        var newConfirmation = PasswordResetConfirmationEntity.Create(userId);
        dbContext.Add(newConfirmation);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new SendPasswordResetConfirmationMessage(username, email, newConfirmation.Code), cancellationToken);
        }, cancellationToken);
    }
}