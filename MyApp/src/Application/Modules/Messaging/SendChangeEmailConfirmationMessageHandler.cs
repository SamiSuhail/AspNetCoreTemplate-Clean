﻿using MediatR;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Presentation.Interfaces.Messaging;
using static MyApp.Presentation.Interfaces.Email.ChangeEmailConstants;

namespace MyApp.Application.Modules.Messaging;

public class SendChangeEmailConfirmationMessageHandler(IEmailSender emailSender) : IRequestHandler<SendChangeEmailConfirmationMessage>
{
    public async Task Handle(SendChangeEmailConfirmationMessage request, CancellationToken cancellationToken)
    {
        var (username, oldEmail, newEmail, oldEmailCode, newEmailCode) = request;

        var messageTextOldEmail = Message(oldEmailCode);
        await emailSender.Send(username, oldEmail, Subject(username), messageTextOldEmail, cancellationToken);

        var messageTextNewEmail = Message(newEmailCode);
        await emailSender.Send(username, newEmail, Subject(username), messageTextNewEmail, cancellationToken);
    }
}
