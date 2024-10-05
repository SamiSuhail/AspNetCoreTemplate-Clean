using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Auth.UserConfirmation.Failures;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Commands.Auth.Registration.ConfirmUserRegistration;

namespace MyApp.Application.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationCommandHandler(IScopedDbContext dbContext) : IRequestHandler<ConfirmUserRegistrationRequest>
{
    public async Task Handle(ConfirmUserRegistrationRequest request, CancellationToken cancellationToken)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);
        var userConfirmation = await dbContext.Set<UserConfirmationEntity>()
            .Include(uc => uc.User)
            .IgnoreQueryFilters()
            .Where(uc => uc.Code == request.Code
                && uc.User.IsEmailConfirmed == false
                && uc.CreatedAt > expirationTime)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw UserConfirmationCodeInvalidFailure.Exception();
        var user = userConfirmation.User;

        user.ConfirmUserRegistration();
        dbContext.Remove(userConfirmation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
