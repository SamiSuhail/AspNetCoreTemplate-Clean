using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Handlers.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationValidator : AbstractValidator<ConfirmUserRegistrationRequest>
{
    public ConfirmUserRegistrationValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .Length(BaseConfirmationConstants.CodeLength);
    }
}
