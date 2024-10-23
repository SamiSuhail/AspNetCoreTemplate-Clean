using MediatR;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Presentation.Interfaces.Messaging;
using static MyApp.Presentation.Interfaces.Email.PasswordResetConstants;

namespace MyApp.Application.Modules.Messaging;

public class PasswordResetSendConfirmationMessageHandler(IEmailSender emailSender) : IRequestHandler<PasswordResetSendConfirmationMessage>
{
    public async Task Handle(PasswordResetSendConfirmationMessage request, CancellationToken cancellationToken)
    {
        var (username, email, code) = request;
        var messageText = Message(code);
        await emailSender.Send(username, email, Subject(username), messageText, cancellationToken);
    }
}
