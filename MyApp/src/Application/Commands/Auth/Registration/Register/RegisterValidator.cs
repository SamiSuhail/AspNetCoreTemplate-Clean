using FluentValidation;
using MyApp.Application.Interfaces.Commands.Auth.Registration.Register;

namespace MyApp.Application.Commands.Auth.Registration.Register;

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
