using MassTransit;
using MyApp.Application.Infrastructure.Abstractions;
using static MyApp.Application.Interfaces.Email.EmailConstants.ChangePassword;

namespace MyApp.Application.Handlers.Commands.UserManagement.PasswordUpdate.ChangePassword;

public record ChangePasswordMessage(string Username, string Email, string ConfirmationCode);
public class ChangePasswordConsumer(IEmailSender emailSender) : IConsumer<ChangePasswordMessage>
{
    public async Task Consume(ConsumeContext<ChangePasswordMessage> context)
    {
        var (username, email, code) = context.Message;
        var messageText = Message(code);
        await emailSender.Send(username, email, Subject(username), messageText, context.CancellationToken);
    }
}
