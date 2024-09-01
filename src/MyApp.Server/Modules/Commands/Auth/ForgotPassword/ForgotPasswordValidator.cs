using FluentValidation;

namespace MyApp.Server.Modules.Commands.Auth.ForgotPassword;

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
