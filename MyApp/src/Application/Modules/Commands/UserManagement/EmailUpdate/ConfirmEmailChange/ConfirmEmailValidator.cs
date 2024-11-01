using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Modules.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

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
