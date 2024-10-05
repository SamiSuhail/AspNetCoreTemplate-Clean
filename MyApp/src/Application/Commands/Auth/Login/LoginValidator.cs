using FluentValidation;
using MyApp.Application.Interfaces.Commands.Auth.Login;

namespace MyApp.Application.Commands.Auth.Login;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(r => r.Username)
            .Username();
        RuleFor(r => r.Password)
            .Password();
    }
}
