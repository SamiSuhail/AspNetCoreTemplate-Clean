using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace MyApp.Server.Modules.Commands.Auth.BackgroundJobs.CleanupConfirmations;

public class CleanupConfirmationsBackgroundJob(IScopedDbContext dbContext) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var emailConfirmationExpirationTime = DateTime.UtcNow.AddMinutes(-EmailConfirmationConstants.ExpirationTimeMinutes);
        var cancellationToken = context.CancellationToken;

        await dbContext.Set<EmailConfirmationEntity>()
            .Where(ec => ec.CreatedAt < emailConfirmationExpirationTime)
            .ExecuteDeleteAsync(cancellationToken);

        var passwordResetExpirationTime = DateTime.UtcNow.AddMinutes(-PasswordResetConfirmationConstants.ExpirationTimeMinutes);

        await dbContext.Set<PasswordResetConfirmationEntity>()
            .Where(ec => ec.CreatedAt < passwordResetExpirationTime)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
