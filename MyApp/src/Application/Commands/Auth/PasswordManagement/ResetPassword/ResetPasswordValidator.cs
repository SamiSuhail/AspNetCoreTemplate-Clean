using FluentValidation;
using MyApp.Application.Interfaces.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Commands.Auth.PasswordManagement.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .Length(BaseConfirmationConstants.CodeLength);

        RuleFor(r => r.Password)
            .Password();
    }
}
