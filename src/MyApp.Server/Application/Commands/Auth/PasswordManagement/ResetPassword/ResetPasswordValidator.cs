using FluentValidation;
using MyApp.Server.Domain.Shared.Confirmations;

namespace MyApp.Server.Application.Commands.Auth.PasswordManagement.ResetPassword;

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
