using FluentValidation;
using MyApp.Application.Interfaces.Commands.Auth.Registration.ResendConfirmation;

namespace MyApp.Application.Handlers.Commands.Auth.Registration.ResendConfirmation;

public class ResendConfirmationValidator : AbstractValidator<ResendConfirmationRequest>
{
    public ResendConfirmationValidator()
    {
        RuleFor(r => r.Email)
            .NotNull()
            .EmailAddress();
    }
}
