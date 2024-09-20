﻿using FluentValidation;
using MyApp.Server.Domain.Shared.Confirmations;

namespace MyApp.Server.Application.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

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