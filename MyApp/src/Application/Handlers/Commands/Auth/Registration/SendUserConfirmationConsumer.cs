using MassTransit;
using MyApp.Application.Infrastructure.Abstractions;
using static MyApp.Application.Interfaces.Email.EmailConstants.SendUserConfirmation;

namespace MyApp.Application.Handlers.Commands.Auth.Registration;

public record SendUserConfirmationMessage(string Username, string Email, string Code);

public class SendUserConfirmationConsumer(IEmailSender emailSender) : IConsumer<SendUserConfirmationMessage>
{
    public async Task Consume(ConsumeContext<SendUserConfirmationMessage> context)
    {
        var (username, email, code) = context.Message;
        var messageText = Message(code);
        await emailSender.Send(username, email, Subject(username), messageText, context.CancellationToken);
    }
}
