using FluentValidation;
using MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ChangeEmail;

namespace MyApp.Application.Commands.UserManagement.EmailUpdate.ChangeEmail;

public class ChangeEmailValidator : AbstractValidator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .EmailAddress();
    }
}
