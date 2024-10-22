using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.RefreshToken;

namespace MyApp.Application.Handlers.Commands.Auth.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(r => r.AccessToken)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.RefreshToken)
            .NotNull()
            .NotEmpty();
    }
}
