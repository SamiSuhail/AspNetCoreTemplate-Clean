using MassTransit;
using MyApp.Application.Infrastructure.Abstractions;
using static MyApp.Application.Interfaces.Email.EmailConstants.ChangeEmail;

namespace MyApp.Application.Handlers.Commands.UserManagement.EmailUpdate.ChangeEmail;

public record ChangeEmailMessage(string Username, string OldEmail, string NewEmail, string OldEmailCode, string NewEmailCode);

public class ChangeEmailConsumer(IEmailSender emailSender) : IConsumer<ChangeEmailMessage>
{
    public async Task Consume(ConsumeContext<ChangeEmailMessage> context)
    {
        var (username, oldEmail, newEmail, oldEmailCode, newEmailCode) = context.Message;

        var messageTextOldEmail = Message(oldEmailCode);
        await emailSender.Send(username, oldEmail, Subject(username), messageTextOldEmail, context.CancellationToken);

        var messageTextNewEmail = Message(newEmailCode);
        await emailSender.Send(username, newEmail, Subject(username), messageTextNewEmail, context.CancellationToken);
    }
}
