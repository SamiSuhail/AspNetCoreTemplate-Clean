using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Domain.Access.Scope;

namespace MyApp.Application.Modules.Commands.Auth.Login;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(r => r.Username)
            .Username();
        RuleFor(r => r.Password)
            .Password();

        RuleForEach(r => r.Scopes)
            .ForEach(c => c.NotEqual(ScopeCollection.Separator))
            .WithMessage($"{{PropertyName}} cannot contains character {ScopeCollection.Separator}");
    }
}
