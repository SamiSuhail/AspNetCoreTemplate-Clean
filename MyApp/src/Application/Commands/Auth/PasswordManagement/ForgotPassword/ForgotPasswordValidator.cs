﻿using FluentValidation;

namespace MyApp.Application.Commands.Auth.PasswordManagement.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordValidator()
    {
        RuleFor(r => r.Username)
            .NotNull()
            .Username();

        RuleFor(r => r.Email)
            .NotNull()
            .EmailAddress();
    }
}