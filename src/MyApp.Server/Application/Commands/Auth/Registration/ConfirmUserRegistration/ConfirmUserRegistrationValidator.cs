using FluentValidation;
using MyApp.Server.Domain.Shared.Confirmations;

namespace MyApp.Server.Application.Commands.Auth.Registration.ConfirmUserRegistration;

public class ConfirmUserRegistrationValidator : AbstractValidator<ConfirmUserRegistrationRequest>
{
    public ConfirmUserRegistrationValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .Length(BaseConfirmationConstants.CodeLength);
    }
}
