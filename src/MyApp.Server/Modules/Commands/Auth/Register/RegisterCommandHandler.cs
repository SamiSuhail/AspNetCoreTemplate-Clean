using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Modules.Commands.Auth.BackgroundJobs.ConfirmRegistration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Server.Modules.Commands.Auth.Register;

public record RegisterRequest(
    string Email,
    string Username,
    string Password) : IRequest;

public class RegisterCommandHandler(IScopedDbContext dbContext, IConfirmRegistrationEmailNotifier notifier) : IRequestHandler<RegisterRequest>
{
    public async Task Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var users = dbContext.Set<UserEntity>()
            .IgnoreQueryFilters();

        var usernameTaken = await users.AnyAsync(u => u.Username == request.Username, cancellationToken);
        var emailTaken = await users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        RegisterConflictFailure.Create(usernameTaken, emailTaken)
            .ThrowOnError();

        var emailConfirmation = EmailConfirmationEntity.Create();
        var user = UserEntity.Create(request.Username, request.Password, request.Email, emailConfirmation);
        dbContext.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        await notifier.StartInBackground(new(user.Username, user.Email, emailConfirmation.Code), cancellationToken);
    }
}
