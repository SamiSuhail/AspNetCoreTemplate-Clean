namespace MyApp.Application.Infrastructure.Abstractions;

public interface IEmailSender
{
    Task Send(string name, string email, string subject, string text, CancellationToken cancellationToken);
}
