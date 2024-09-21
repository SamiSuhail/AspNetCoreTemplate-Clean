namespace MyApp.Server.Infrastructure.Abstractions;

public interface IEmailSender
{
    Task Send(string name, string email, string subject, string text, CancellationToken cancellationToken);
}
