using FluentValidation;
using MyApp.Server.Domain.Auth.EmailConfirmation;

namespace MyApp.Server.Modules.Commands.Auth.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .Length(EmailConfirmationConstants.CodeLength);
    }
}
