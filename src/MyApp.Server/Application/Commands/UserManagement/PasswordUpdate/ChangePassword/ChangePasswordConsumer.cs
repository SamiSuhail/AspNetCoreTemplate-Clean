using MassTransit;
using MyApp.Server.Domain.Shared.Confirmations;
using MyApp.Application.Infrastructure.Abstractions;

namespace MyApp.Server.Application.Commands.UserManagement.PasswordUpdate.ChangePassword;

public record ChangePasswordMessage(string Username, string Email, string ConfirmationCode);
public class ChangePasswordConsumer(IEmailSender emailSender) : IConsumer<ChangePasswordMessage>
{
    private const string MessageSubject = "Go2gether Password Change";
    private const string MessageTemplate = """
        Please use the code below to confirm your password change request. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public async Task Consume(ConsumeContext<ChangePasswordMessage> context)
    {
        var (username, email, code) = context.Message;

        var messageTextOldEmail = string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, code);
        await emailSender.Send(username, email, MessageSubject, messageTextOldEmail, context.CancellationToken);
    }
}
