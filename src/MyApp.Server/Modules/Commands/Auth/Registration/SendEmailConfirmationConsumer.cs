using MassTransit;
using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Infrastructure.Email;

namespace MyApp.Server.Modules.Commands.Auth.Registration;

public record SendEmailConfirmationMessage(string Username, string Email, string Code);

public class SendEmailConfirmationConsumer(IEmailSender emailSender) : IConsumer<SendEmailConfirmationMessage>
{
    private const string MessageTemplate = """
        Please use the code below to confirm your e-mail address. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public async Task Consume(ConsumeContext<SendEmailConfirmationMessage> context)
    {
        var (username, email, code) = context.Message;
        await Task.Delay(TimeSpan.FromSeconds(15));
        var messageText = string.Format(MessageTemplate, EmailConfirmationConstants.ExpirationTimeMinutes, code);
        await emailSender.Send(username, email, "Go2Gether Email Confirmation", messageText, context.CancellationToken);
    }
}
