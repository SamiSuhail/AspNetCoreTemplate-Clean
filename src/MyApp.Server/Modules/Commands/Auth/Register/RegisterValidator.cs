using FluentValidation;

namespace MyApp.Server.Modules.Commands.Auth.Register;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(r => r.Username)
            .Username();
        RuleFor(r => r.Password)
            .Password();

        RuleFor(r => r.Email)
            .NotNull()
            .EmailAddress();
    }
}
