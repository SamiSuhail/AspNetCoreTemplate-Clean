using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Shared;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Application.Commands.Auth.BackgroundJobs.CleanupConfirmations;

public class CleanupConfirmationsRequest() : IRequest;

public class CleanupConfirmationsHandler(IAppDbContextFactory dbContextFactory) : IRequestHandler<CleanupConfirmationsRequest>
{
    public async Task Handle(CleanupConfirmationsRequest request, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            CleanupUserConfirmations(cancellationToken),
            CleanupPasswordResetConfirmations(cancellationToken)
        );
    }

    private async Task CleanupUserConfirmations(CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateTransientDbContextAsync(cancellationToken);
        var expirationTime = DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);
        await dbContext.Set<UserConfirmationEntity>()
            .Where(uc => uc.CreatedAt < expirationTime)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private async Task CleanupPasswordResetConfirmations(CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateTransientDbContextAsync(cancellationToken);
        var expirationTime = DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);

        await dbContext.Set<PasswordResetConfirmationEntity>()
            .Where(prc => prc.CreatedAt < expirationTime)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
