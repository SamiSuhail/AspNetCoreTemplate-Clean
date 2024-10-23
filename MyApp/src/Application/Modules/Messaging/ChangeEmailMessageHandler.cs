using MediatR;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Presentation.Interfaces.Messaging;
using static MyApp.Presentation.Interfaces.Email.ChangeEmailConstants;

namespace MyApp.Application.Modules.Messaging;

public class ChangeEmailMessageHandler(IEmailSender emailSender) : IRequestHandler<ChangeEmailMessage>
{
    public async Task Handle(ChangeEmailMessage request, CancellationToken cancellationToken)
    {
        var (username, oldEmail, newEmail, oldEmailCode, newEmailCode) = request;

        var messageTextOldEmail = Message(oldEmailCode);
        await emailSender.Send(username, oldEmail, Subject(username), messageTextOldEmail, cancellationToken);

        var messageTextNewEmail = Message(newEmailCode);
        await emailSender.Send(username, newEmail, Subject(username), messageTextNewEmail, cancellationToken);
    }
}
