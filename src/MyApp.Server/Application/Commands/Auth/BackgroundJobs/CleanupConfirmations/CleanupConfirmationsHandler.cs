using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Application.Commands.Auth.BackgroundJobs.CleanupConfirmations;

public class CleanupConfirmationsRequest() : IRequest;

public class CleanupConfirmationsHandler(IScopedDbContext dbContext) : IRequestHandler<CleanupConfirmationsRequest>
{
    public async Task Handle(CleanupConfirmationsRequest request, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            CleanupEmailConfirmations(cancellationToken),
            CleanupPasswordResetConfirmations(cancellationToken)
        );
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
