using MassTransit;
using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Infrastructure.Email;

namespace MyApp.Server.Modules.Commands.Auth.ConfirmRegistration;

public record ConfirmRegistrationMessage(string Username, string Email, string Code);

public class ConfirmRegistrationConsumer(IEmailSender emailSender) : IConsumer<ConfirmRegistrationMessage>
{
    private const string MessageTemplate = """
        Please use the code below to confirm your e-mail address. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public async Task Consume(ConsumeContext<ConfirmRegistrationMessage> context)
    {
        var (username, email, code) = context.Message;
        await Task.Delay(TimeSpan.FromSeconds(15));
        var messageText = string.Format(MessageTemplate, EmailConfirmationConstants.ExpirationTimeMinutes, code);
        await emailSender.Send(username, email, "Go2Gether Registration Confirmation", messageText, context.CancellationToken);
    }
}
