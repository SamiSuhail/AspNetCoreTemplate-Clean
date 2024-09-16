using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Server.Domain.Shared.Confirmations;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Application.Commands.Auth.PasswordManagement.ResetPassword;

public record ResetPasswordRequest(string Code, string Password) : IRequest;

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
