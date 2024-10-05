using FluentValidation;
using MyApp.Application.Interfaces.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public class ConfirmPasswordChangeValidator : AbstractValidator<ConfirmPasswordChangeRequest>
{
    public ConfirmPasswordChangeValidator()
    {
        RuleFor(r => r.Code)
            .NotNull()
            .NotEmpty()
            .Length(BaseConfirmationConstants.CodeLength);
    }
}
