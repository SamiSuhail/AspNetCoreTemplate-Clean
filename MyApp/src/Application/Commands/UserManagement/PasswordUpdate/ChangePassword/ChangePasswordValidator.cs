using FluentValidation;
using MyApp.Application.Commands.Auth;

namespace MyApp.Application.Commands.UserManagement.PasswordUpdate.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .Password();
    }
}
