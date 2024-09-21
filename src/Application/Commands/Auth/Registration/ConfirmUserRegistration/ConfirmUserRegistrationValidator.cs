using FluentValidation;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationValidator : AbstractValidator<ConfirmUserRegistrationRequest>
{
    public ConfirmUserRegistrationValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .Length(BaseConfirmationConstants.CodeLength);
    }
}
