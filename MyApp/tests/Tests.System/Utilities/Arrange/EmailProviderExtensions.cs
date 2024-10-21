using MyApp.Domain.Shared.Confirmations;
using MyApp.Tests.System.Providers.Email;
using MyApp.Utilities.Tasks;
using static MyApp.Application.Interfaces.Email.EmailConstants;

namespace MyApp.Tests.System.Utilities.Arrange;

public static class EmailProviderExtensions
{
    private const string CodePrefix = "Code: ";

    public static async Task<string> GetUserConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string username)
        => await emailProvider.GetConfirmationCode(recipientAddress, SendUserConfirmation.Subject(username));
    public static async Task<string> GetPasswordResetConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string username)
        => await emailProvider.GetConfirmationCode(recipientAddress, ForgotPassword.Subject(username));
    public static async Task<string> GetChangePasswordConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string username)
        => await emailProvider.GetConfirmationCode(recipientAddress, ChangePassword.Subject(username));
    public static async Task<string> GetChangeEmailConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string username)
        => await emailProvider.GetConfirmationCode(recipientAddress, ChangeEmail.Subject(username));

    private static async Task<string> GetConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string subject)
    {
        var message = await emailProvider.GetRecentEmailsWhereSubjectContains(subject, recipientAddress)
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
