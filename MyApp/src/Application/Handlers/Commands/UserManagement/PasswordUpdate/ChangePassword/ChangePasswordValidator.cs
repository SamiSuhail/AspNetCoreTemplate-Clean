﻿using FluentValidation;
using MyApp.Application.Handlers.Commands.Auth;
using MyApp.Application.Interfaces.Commands.UserManagement.PasswordUpdate.ChangePassword;

namespace MyApp.Application.Handlers.Commands.UserManagement.PasswordUpdate.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .Password();
    }
}
