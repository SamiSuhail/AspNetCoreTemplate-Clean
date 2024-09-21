using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.EmailChangeConfirmation;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Domain.Shared;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation;
using MyApp.Application.Infrastructure.Abstractions.Database;

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
