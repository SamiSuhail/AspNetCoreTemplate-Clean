using FluentValidation;
using MyApp.Server.Application.Commands.Auth;

namespace MyApp.Server.Application.Commands.UserManagement.PasswordUpdate.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .Password();
    }
}
