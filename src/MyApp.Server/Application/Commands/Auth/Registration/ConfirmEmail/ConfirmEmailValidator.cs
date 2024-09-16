using FluentValidation;
using MyApp.Server.Domain.Shared;

namespace MyApp.Server.Application.Commands.Auth.Registration.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .Length(BaseConfirmationConstants.CodeLength);
    }
}
