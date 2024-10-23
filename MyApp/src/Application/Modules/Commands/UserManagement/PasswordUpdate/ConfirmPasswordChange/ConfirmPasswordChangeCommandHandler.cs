using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Infrastructure.Repositories.PasswordReset;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

namespace MyApp.Application.Modules.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public class ConfirmPasswordChangeCommandHandler(IUserContextAccessor userContextAccessor, IScopedDbContext dbContext) : IRequestHandler<ConfirmPasswordChangeRequest>
{
    public async Task Handle(ConfirmPasswordChangeRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, _, _) = userContextAccessor.UserData;
        var (code, password) = request;
        await dbContext.ConfirmPasswordReset(userId, code, password, cancellationToken);
    }
}
