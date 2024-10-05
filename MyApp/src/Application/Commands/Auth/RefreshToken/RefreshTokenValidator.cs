using FluentValidation;
using MyApp.Application.Interfaces.Commands.Auth.RefreshToken;

namespace MyApp.Application.Commands.Auth.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotNull()
            .NotEmpty();
    }
}
