using MimeKit;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Presentation.Interfaces.Email;
using MyApp.Tests.System.Providers.Email;
using MyApp.Utilities.Streams;
using MyApp.Utilities.Strings;
using MyApp.Utilities.Tasks;

namespace MyApp.Tests.System.Utilities.Arrange;

public static class EmailProviderExtensions
{
    private const string CodePrefix = "Code: ";

    public static async Task<string> GetUserConfirmationCode(this EmailProvider emailProvider, string recipientAddress, string username)
        => await emailProvider.GetConfirmationCode(recipientAddress, RegisterUserConstants.Subject(username));
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

        var messageText = await ReadMessageText(message!);
        return GetCodeFromMessage(messageText);
    }

    private static async Task<string> ReadMessageText(MimeMessage message)
    {
        using var stream = new MemoryStream();
        await message!.Body.WriteToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return await stream.ReadAsStringAsync();
    }

    private static string GetCodeFromMessage(string message)
    {
        var codePrefixIndex = message.IndexOf(CodePrefix);
        codePrefixIndex.Should().BePositive(because: $"The message should contain {CodePrefix}");

        var startIndex = codePrefixIndex + CodePrefix.Length;
        var endIndex = startIndex + BaseConfirmationConstants.CodeLength;
        endIndex.Should().BeLessThanOrEqualTo(message.Length, because: "The code should be located right after the prefix.");

        var code = message[startIndex..endIndex];
        code.Should().HaveLength(BaseConfirmationConstants.CodeLength);
        code.All(c => c.IsNumber()).Should().BeTrue();
        return code;
    }
}
