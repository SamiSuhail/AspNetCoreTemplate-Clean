using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Modules.Commands.Auth.ForgotPassword.EmailNotifier;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Server.Modules.Commands.Auth.ForgotPassword;

public record ForgotPasswordRequest(string Email, string Username) : IRequest;

public class ForgotPasswordCommandHandler(IScopedDbContext dbContext, IForgotPasswordEmailNotifier notifier) : IRequestHandler<ForgotPasswordRequest>
{
    public async Task Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Where(u => u.Email == request.Email && u.Username == request.Username)
            .Select(u => new
            {
                u.Id,
                u.PasswordResetConfirmation,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw ForgotPasswordInvalidFailure.Exception();

        if (user.PasswordResetConfirmation != null)
            dbContext.Remove(user.PasswordResetConfirmation);

        var newConfirmation = PasswordResetConfirmationEntity.Create(user.Id);
        dbContext.Add(newConfirmation);

        await dbContext.SaveChangesAsync(cancellationToken);

        await notifier.StartInBackground(new(request.Username, request.Email, newConfirmation.Code), cancellationToken);
    }
}
