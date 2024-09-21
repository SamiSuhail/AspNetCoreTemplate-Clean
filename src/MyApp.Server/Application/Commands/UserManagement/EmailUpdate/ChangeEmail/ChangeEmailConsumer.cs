using MassTransit;
using MyApp.Server.Domain.Shared.Confirmations;
using MyApp.Application.Infrastructure.Abstractions;

namespace MyApp.Server.Application.Commands.UserManagement.EmailUpdate.ChangeEmail;

public record ChangeEmailMessage(string Username, string OldEmail, string NewEmail, string OldEmailCode, string NewEmailCode);

public class ChangeEmailConsumer(IEmailSender emailSender) : IConsumer<ChangeEmailMessage>
{
    private const string MessageTemplate = """
        Please use the code below to confirm your email change request. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public async Task Consume(ConsumeContext<ChangeEmailMessage> context)
    {
        var (username, oldEmail, newEmail, oldEmailCode, newEmailCode) = context.Message;

        var messageTextOldEmail = string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, oldEmailCode);
        await emailSender.Send(username, oldEmail, "Go2Gether Email Change Confirmation", messageTextOldEmail, context.CancellationToken);

        var messageTextNewEmail = string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, newEmailCode);
        await emailSender.Send(username, newEmail, "Go2Gether Email Change Confirmation", messageTextNewEmail, context.CancellationToken);
    }
}
