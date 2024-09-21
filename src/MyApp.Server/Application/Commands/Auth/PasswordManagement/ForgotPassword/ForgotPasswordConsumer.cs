using MassTransit;
using MyApp.Server.Domain.Shared.Confirmations;
using MyApp.Server.Infrastructure.Abstractions;

namespace MyApp.Server.Application.Commands.Auth.PasswordManagement.ForgotPassword;

public record ForgotPasswordMessage(string Username, string Email, string Code);

public class ForgotPasswordConsumer(IEmailSender emailSender) : IConsumer<ForgotPasswordMessage>
{
    public const string MessageTemplate = """
        Please use the code below to reset your password. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public async Task Consume(ConsumeContext<ForgotPasswordMessage> context)
    {
        var (username, email, code) = context.Message;
        var messageText = string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, code);
        await emailSender.Send(username, email, "Go2Gether Password Reset", messageText, context.CancellationToken);
    }
}
