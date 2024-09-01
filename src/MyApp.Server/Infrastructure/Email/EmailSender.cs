using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace MyApp.Server.Infrastructure.Email;

public interface IEmailSender
{
    Task Send(string name, string email, string subject, string text, CancellationToken cancellationToken);
}

public class EmailSender : IEmailSender
{
    const string SenderName = "MyApp";
    const string SenderEmail = "myapp@gmail.com";
    const string SMTPHost = "localhost";
    const int SMTPPort = 1025;
    public async Task Send(string name, string email, string subject, string text, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(SenderName, SenderEmail));
        message.To.Add(new MailboxAddress(name, email));

        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html)
        {
            Text = text
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(SMTPHost, SMTPPort, false, cancellationToken);

        await smtp.SendAsync(message, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
