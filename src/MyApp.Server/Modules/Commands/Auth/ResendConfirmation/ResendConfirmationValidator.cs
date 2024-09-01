﻿using FluentValidation;

namespace MyApp.Server.Modules.Commands.Auth.ResendConfirmation;

public class ResendConfirmationValidator : AbstractValidator<ResendConfirmationRequest>
{
    public ResendConfirmationValidator()
    {
        RuleFor(r => r.Email)
            .NotNull()
            .EmailAddress();
    }
}