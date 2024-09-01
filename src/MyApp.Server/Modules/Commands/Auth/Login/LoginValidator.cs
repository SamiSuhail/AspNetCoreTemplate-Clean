using FluentValidation;

namespace MyApp.Server.Modules.Commands.Auth.Login;

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
