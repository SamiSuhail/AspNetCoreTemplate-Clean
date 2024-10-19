using MassTransit;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Application.Infrastructure.Abstractions;

namespace MyApp.Application.Handlers.Commands.Auth.Registration;

public record SendUserConfirmationMessage(string Username, string Email, string Code);

public class SendUserConfirmationConsumer(IEmailSender emailSender) : IConsumer<SendUserConfirmationMessage>
{
    private const string MessageTemplate = """
        Please use the code below to confirm your user account. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public async Task Consume(ConsumeContext<SendUserConfirmationMessage> context)
    {
        var (username, email, code) = context.Message;
        var messageText = string.Format(MessageTemplate, BaseConfirmationConstants.ExpirationTimeMinutes, code);
        await emailSender.Send(username, email, "Go2Gether User Confirmation", messageText, context.CancellationToken);
    }
}
