using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.EmailConfirmation.Failures;
using MyApp.Server.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Server.Modules.Commands.Auth.Registration.ConfirmEmail;

public record ConfirmEmailRequest(string Code) : IRequest;

public class ConfirmEmailCommandHandler(IScopedDbContext dbContext) : IRequestHandler<ConfirmEmailRequest>
{
    public async Task Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(-EmailConfirmationConstants.ExpirationTimeMinutes);
        var emailConfirmation = await dbContext.Set<EmailConfirmationEntity>()
            .Include(ec => ec.User)
            .IgnoreQueryFilters()
            .Where(ec => ec.Code == request.Code
                && ec.User.IsEmailConfirmed == false
                && ec.CreatedAt > expirationTime)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw EmailConfirmationCodeInvalidFailure.Exception();
        var user = emailConfirmation.User;

        user.ConfirmEmail();
        dbContext.Remove(emailConfirmation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
