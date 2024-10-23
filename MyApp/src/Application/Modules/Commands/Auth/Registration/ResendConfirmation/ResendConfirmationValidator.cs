using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ResendConfirmation;

namespace MyApp.Application.Modules.Commands.Auth.Registration.ResendConfirmation;

public class ResendConfirmationValidator : AbstractValidator<ResendConfirmationRequest>
{
    public ResendConfirmationValidator()
    {
        RuleFor(r => r.Email)
            .NotNull()
            .EmailAddress();
    }
}
