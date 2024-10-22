using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;

namespace MyApp.Application.Handlers.Commands.Auth.PasswordManagement.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordValidator()
    {
        RuleFor(r => r.Username)
            .NotNull()
            .Username();

        RuleFor(r => r.Email)
            .NotNull()
            .EmailAddress();
    }
}
