using MimeKit;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Presentation.Interfaces.Email;
using MyApp.Tests.System.Providers.Email;
using MyApp.Utilities.Streams;
using MyApp.Utilities.Tasks;

namespace MyApp.Tests.System.Utilities.Arrange;

public static class EmailProviderExtensions
{
    private const string CodePrefix = "Code: ";

    public static async Task<string> GetUserConfirmationCode(
        this EmailProvider emailProvider, 
        string username,
        Func<EmailUsersSettings, EmailUser>? userSelector = null)
        => await emailProvider.GetConfirmationCode(RegisterUserConstants.Subject(username), userSelector);

    public static async Task<string> GetPasswordResetConfirmationCode(
        this EmailProvider emailProvider, 
        string username,
        Func<EmailUsersSettings, EmailUser>? userSelector = null)
        => await emailProvider.GetConfirmationCode(PasswordResetConstants.Subject(username), userSelector);

    public static async Task<string> GetChangeEmailConfirmationCode(
        this EmailProvider emailProvider, 
        string username,
        Func<EmailUsersSettings, EmailUser>? userSelector = null)
        => await emailProvider.GetConfirmationCode(ChangeEmailConstants.Subject(username), userSelector);

    private static async Task<string> GetConfirmationCode(
        this EmailProvider emailProvider, 
        string subject,
        Func<EmailUsersSettings, EmailUser>? userSelector = null)
    {
        var message = await emailProvider.GetMessageOrDefault(subject, userSelector);

        var counter = 0;
        while (message == null && counter++ < 5)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            message = await emailProvider.GetMessageOrDefault(subject, userSelector);
        }

        message.Should().NotBeNull();

        var messageText = await ReadMessageText(message!);
        return GetCodeFromMessage(messageText);
    }

    private static async Task<MimeMessage?> GetMessageOrDefault(
        this EmailProvider emailProvider, 
        string subject,
        Func<EmailUsersSettings, EmailUser>? userSelector = null)
    {
        return await emailProvider.GetEmails(inbox =>
                inbox.GetRecentEmailsWhereSubjectContains(subject), userSelector)
            .FirstOrDefaultAsync();
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
        code.AssertValidConfirmationCode();
        return code;
    }
}
