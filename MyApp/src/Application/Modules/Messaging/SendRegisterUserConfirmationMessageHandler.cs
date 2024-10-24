using MediatR;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Presentation.Interfaces.Messaging;
using static MyApp.Presentation.Interfaces.Email.RegisterUserConstants;

namespace MyApp.Application.Modules.Messaging;

public class SendRegisterUserConfirmationMessageHandler(IEmailSender emailSender) : IRequestHandler<SendRegisterUserConfirmationMessage>
{
    public async Task Handle(SendRegisterUserConfirmationMessage request, CancellationToken cancellationToken)
    {
        var (username, email, code) = request;
        var messageText = Message(code);
        await emailSender.Send(username, email, Subject(username), messageText, cancellationToken);
    }
}
