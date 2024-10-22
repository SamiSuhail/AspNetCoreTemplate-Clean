using FluentValidation;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ChangeEmail;

namespace MyApp.Application.Handlers.Commands.UserManagement.EmailUpdate.ChangeEmail;

public class ChangeEmailValidator : AbstractValidator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .EmailAddress();
    }
}
