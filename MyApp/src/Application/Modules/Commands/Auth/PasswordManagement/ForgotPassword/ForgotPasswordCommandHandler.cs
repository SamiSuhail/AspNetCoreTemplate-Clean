using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Infrastructure.Repositories.PasswordReset;
using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Domain.Auth.User;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;

namespace MyApp.Application.Modules.Commands.Auth.PasswordManagement.ForgotPassword;

public class ForgotPasswordCommandHandler(IScopedDbContext dbContext, IPasswordResetRepository passwordResetRepository) : IRequestHandler<ForgotPasswordRequest>
{
    public async Task Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var (email, username) = request;

        var user = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Where(u => u.Email == email && u.Username == username)
            .Select(u => new
            {
                u.Id,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw ForgotPasswordInvalidFailure.Exception();

        await passwordResetRepository.SendConfirmation(user.Id, username, email, cancellationToken);
    }
}
