using FluentValidation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;

namespace MyApp.Server.Modules.Commands.Auth.PasswordManagement.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .Length(PasswordResetConfirmationConstants.CodeLength);

        RuleFor(r => r.Password)
            .Password();
    }
}
