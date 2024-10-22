using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ResetPassword;

namespace MyApp.Application.Handlers.Commands.Auth.PasswordManagement.ResetPassword;

public class ResetPasswordCommandHandler(IScopedDbContext dbContext) : IRequestHandler<ResetPasswordRequest>
{
    public async Task Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);
        var passwordResetConfirmation = await dbContext.Set<PasswordResetConfirmationEntity>()
            .IgnoreQueryFilters()
            .Include(p => p.User)
            .Where(p => p.Code == request.Code && p.CreatedAt > expirationTime)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw PasswordResetConfirmationCodeInvalidFailure.Exception();

        dbContext.Remove(passwordResetConfirmation);
        passwordResetConfirmation.User.UpdatePassword(request.Password);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
