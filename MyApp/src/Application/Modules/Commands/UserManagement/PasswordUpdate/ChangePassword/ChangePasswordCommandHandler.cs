using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Repositories.PasswordReset;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ChangePassword;

namespace MyApp.Application.Modules.Commands.UserManagement.PasswordUpdate.ChangePassword;

public class ChangePasswordCommandHandler(
    IUserContextAccessor userContextAccessor,
    IPasswordResetRepository passwordResetRepository
    ) : IRequestHandler<ChangePasswordRequest>
{
    public async Task Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var (userId, username, email, _) = userContextAccessor.UserData;
        await passwordResetRepository.SendConfirmation(userId, username, email, cancellationToken);
    }
}
