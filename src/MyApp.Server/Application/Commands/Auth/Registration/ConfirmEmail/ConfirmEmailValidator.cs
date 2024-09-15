using FluentValidation;
using MyApp.Server.Domain.Auth.EmailConfirmation;

namespace MyApp.Server.Application.Commands.Auth.Registration.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .Length(EmailConfirmationConstants.CodeLength);
    }
}
