using MassTransit;
using MyApp.Application.Infrastructure.Abstractions;
using static MyApp.Presentation.Interfaces.Email.ForgotPasswordConstants;

namespace MyApp.Application.Handlers.Commands.Auth.PasswordManagement.ForgotPassword;

public record ForgotPasswordMessage(string Username, string Email, string Code);

public class ForgotPasswordConsumer(IEmailSender emailSender) : IConsumer<ForgotPasswordMessage>
{
    public async Task Consume(ConsumeContext<ForgotPasswordMessage> context)
    {
        var (username, email, code) = context.Message;
        var messageText = Message(code);
        await emailSender.Send(username, email, Subject(username), messageText, context.CancellationToken);
    }
}
