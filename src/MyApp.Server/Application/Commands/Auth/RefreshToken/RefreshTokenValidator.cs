using FluentValidation;

namespace MyApp.Server.Application.Commands.Auth.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotNull()
            .NotEmpty();
    }
}
