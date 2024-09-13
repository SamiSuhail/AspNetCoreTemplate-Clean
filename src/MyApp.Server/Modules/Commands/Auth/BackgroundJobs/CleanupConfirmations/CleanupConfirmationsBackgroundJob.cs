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
        var cancellationToken = context.CancellationToken;

        await CleanupEmailConfirmations(cancellationToken);
        await CleanupPasswordResetConfirmations(cancellationToken);
    }

    private async Task CleanupEmailConfirmations(CancellationToken cancellationToken)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(-EmailConfirmationConstants.ExpirationTimeMinutes);
        await dbContext.Set<EmailConfirmationEntity>()
            .Where(ec => ec.CreatedAt < expirationTime)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private async Task CleanupPasswordResetConfirmations(CancellationToken cancellationToken)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(-PasswordResetConfirmationConstants.ExpirationTimeMinutes);

        await dbContext.Set<PasswordResetConfirmationEntity>()
            .Where(ec => ec.CreatedAt < expirationTime)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
