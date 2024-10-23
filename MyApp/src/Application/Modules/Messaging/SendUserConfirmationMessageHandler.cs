using MediatR;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Presentation.Interfaces.Messaging;
using static MyApp.Presentation.Interfaces.Email.SendUserConfirmationConstants;

namespace MyApp.Application.Modules.Messaging;

public class SendUserConfirmationMessageHandler(IEmailSender emailSender) : IRequestHandler<SendUserConfirmationMessage>
{
    public async Task Handle(SendUserConfirmationMessage request, CancellationToken cancellationToken)
    {
        var (username, email, code) = request;
        var messageText = Message(code);
        await emailSender.Send(username, email, Subject(username), messageText, cancellationToken);
    }
}
