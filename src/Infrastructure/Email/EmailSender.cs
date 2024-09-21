using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MyApp.Application.Infrastructure.Abstractions;

namespace MyApp.Infrastructure.Email;

public class EmailSender(EmailSettings settings) : IEmailSender
{
    public async Task Send(string name, string email, string subject, string text, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
        message.To.Add(new MailboxAddress(name, email));

        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = text
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(settings.SMTPHost, settings.SMTPPort, false, cancellationToken);

        await smtp.SendAsync(message, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
