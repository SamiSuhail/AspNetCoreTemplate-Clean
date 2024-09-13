using MassTransit;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Infrastructure.Email;

namespace MyApp.Server.Modules.Commands.Auth.ForgotPassword;

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
        await Task.Delay(TimeSpan.FromSeconds(15));
        var messageText = string.Format(MessageTemplate, PasswordResetConfirmationConstants.ExpirationTimeMinutes, code);
        await emailSender.Send(username, email, "Go2Gether Password Reset", messageText, context.CancellationToken);
    }
}
