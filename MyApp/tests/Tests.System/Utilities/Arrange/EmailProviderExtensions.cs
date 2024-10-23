using MyApp.Presentation.Interfaces.Email;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Tests.System.Providers.Email;
using MyApp.Utilities.Tasks;

namespace MyApp.Tests.System.Utilities.Arrange;

public static class EmailProviderExtensions
{
    private const string CodePrefix = "Code: ";

    public static async Task<string> GetUserConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string username)
        => await emailProvider.GetConfirmationCode(recipientAddress, SendUserConfirmationConstants.Subject(username));
    public static async Task<string> GetPasswordResetConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string username)
        => await emailProvider.GetConfirmationCode(recipientAddress, PasswordResetConstants.Subject(username));
    public static async Task<string> GetChangeEmailConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string username)
        => await emailProvider.GetConfirmationCode(recipientAddress, ChangeEmailConstants.Subject(username));

    private static async Task<string> GetConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string subject)
    {
        var message = await emailProvider.GetEmails(inbox => 
                inbox.GetRecentEmailsWhereSubjectContains(subject, recipientAddress))
            .FirstOrDefaultAsync();
        message.Should().NotBeNull();
        return GetCodeFromMessage(message!.TextBody);
    }

    private static string GetCodeFromMessage(string message)
    {
        var startIndex = message.IndexOf(CodePrefix) + CodePrefix.Length;
        startIndex.Should().BePositive(because: $"The message should contain {CodePrefix}");
        var endIndex = startIndex + BaseConfirmationConstants.CodeLength;
        return message[startIndex..endIndex];
    }
}
