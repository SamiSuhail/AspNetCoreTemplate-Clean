using FluentValidation;

namespace MyApp.Application.Modules.Commands.UserManagement.EmailUpdate.ChangeEmail;

public class ChangeEmailValidator : AbstractValidator<Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ChangeEmail.ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .EmailAddress();
    }
}
