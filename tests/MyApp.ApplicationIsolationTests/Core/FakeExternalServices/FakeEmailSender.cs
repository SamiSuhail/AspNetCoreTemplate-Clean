using MyApp.Server.Infrastructure.Email;

namespace MyApp.ApplicationIsolationTests.Core.FakeExternalServices;

internal class FakeEmailSender : IEmailSender
{
    public Task Send(string name, string email, string subject, string text, CancellationToken cancellationToken)
        => throw new Exception("Application Isolation Tests should not be sending emails! Check your mocks!");
}
