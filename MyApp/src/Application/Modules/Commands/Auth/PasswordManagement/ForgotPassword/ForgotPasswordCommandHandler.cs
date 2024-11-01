using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Infrastructure.Repositories.Instance;
using MyApp.Application.Infrastructure.Repositories.PasswordReset;
using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Domain.Auth.User;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;

namespace MyApp.Application.Modules.Commands.Auth.PasswordManagement.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordRequest Request, string? InstanceName) : IRequest;

public class ForgotPasswordCommandHandler(
    IScopedDbContext dbContext,
    IPasswordResetRepository passwordResetRepository
    ) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var (request, instanceName) = command;
        var instanceId = await dbContext.GetInstanceId(instanceName, cancellationToken);
        var (email, username) = request;

        var user = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Where(u => u.Email == email && u.Username == username && u.InstanceId == instanceId)
            .Select(u => new
            {
                u.Id,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw ForgotPasswordUserNotFoundFailure.Exception();

        await passwordResetRepository.SendConfirmation(user.Id, username, email, cancellationToken);
    }
}
