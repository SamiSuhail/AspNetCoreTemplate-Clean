using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Modules.Commands.Auth.PasswordManagement.ResetPassword;

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
