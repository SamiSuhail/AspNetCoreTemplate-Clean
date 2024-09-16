using FluentValidation;
using MyApp.Server.Domain.Shared.Confirmations;

namespace MyApp.Server.Application.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

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
