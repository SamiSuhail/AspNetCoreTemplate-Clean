using FluentValidation;
using MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Handlers.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailChangeRequest>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.OldEmailCode)
            .NotNull()
            .Length(BaseConfirmationConstants.CodeLength);

        RuleFor(x => x.NewEmailCode)
            .NotNull()
            .Length(BaseConfirmationConstants.CodeLength);
    }
}
