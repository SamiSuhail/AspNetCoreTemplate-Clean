using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Application.Modules.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

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
