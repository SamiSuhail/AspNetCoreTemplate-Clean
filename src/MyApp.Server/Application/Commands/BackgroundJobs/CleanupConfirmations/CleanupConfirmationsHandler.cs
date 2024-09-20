using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Domain.Auth.EmailChangeConfirmation;
using MyApp.Server.Domain.Shared.Confirmations;
using MyApp.Server.Domain.Shared;
using MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation;

namespace MyApp.Server.Application.Commands.BackgroundJobs.CleanupConfirmations;

public class CleanupConfirmationsRequest() : IRequest;

public class CleanupConfirmationsHandler(IAppDbContextFactory dbContextFactory) : IRequestHandler<CleanupConfirmationsRequest>
{
    public async Task Handle(CleanupConfirmationsRequest request, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            CleanupConfirmations<UserConfirmationEntity>(cancellationToken),
            CleanupConfirmations<PasswordResetConfirmationEntity>(cancellationToken),
            CleanupConfirmations<EmailChangeConfirmationEntity>(cancellationToken),
            CleanupConfirmations<PasswordChangeConfirmationEntity>(cancellationToken)
        );
    }

    private async Task CleanupConfirmations<TConfirmationEntity>(CancellationToken cancellationToken) where TConfirmationEntity : class, ICreationAudited
    {
        await using var dbContext = await dbContextFactory.CreateTransientDbContextAsync(cancellationToken);
        var expirationTime = DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);
        await dbContext.Set<TConfirmationEntity>()
            .Where(uc => uc.CreatedAt < expirationTime)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
