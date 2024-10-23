using MediatR;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Application.Infrastructure.Repositories.PasswordReset;

namespace MyApp.Application.Modules.Commands.Auth.PasswordManagement.ResetPassword;

public class ResetPasswordCommandHandler(IScopedDbContext dbContext) : IRequestHandler<ResetPasswordRequest>
{
    public async Task Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var (code, password) = request;
        var passwordReset = await dbContext.Set<PasswordResetConfirmationEntity>()
            .IgnoreQueryFilters()
            .Where(pr => pr.Code == code)
            .Select(pr => new
            {
                pr.UserId
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw PasswordResetConfirmationCodeInvalidFailure.Exception();

        await dbContext.ConfirmPasswordReset(passwordReset.UserId, code, password, cancellationToken);
    }
}
