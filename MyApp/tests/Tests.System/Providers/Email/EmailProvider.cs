using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using MyApp.Utilities.Tasks;

namespace MyApp.Tests.System.Providers.Email;

public class EmailProvider(EmailSettings settings)
{
    public async IAsyncEnumerable<MimeMessage> GetEmails(Func<IMailFolder, Task<IEnumerable<UniqueId>>> filter)
    {
        using var client = new ImapClient();

        await client.ConnectAsync(settings.Host, settings.Port, useSsl: false);

        if (settings.Auth is { } authSettings)
            await client.AuthenticateAsync(authSettings.Username, authSettings.Password);

        var inbox = client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadOnly);

        var uids = await filter(inbox);

        var messages = uids.WhenAllParallelized(async uid =>
        {
            var message = await inbox.GetMessageAsync(uid);
            return message;
        });

        await foreach (var message in messages)
            yield return message;

        await inbox.CloseAsync();
        await client.DisconnectAsync(true);
    }
}

public static class EmailProviderExtensions
{
    public static async IAsyncEnumerable<MimeMessage> GetRecentEmailsWhereSubjectContains(
        this EmailProvider emailProvider,
        string subjectFilter, 
        string recipientFilter, 
        int sentSinceMinutesFilter = 5)
    {
        var results = emailProvider.GetEmails(async inbox =>
        {
            var uids = await inbox.SearchAsync(SearchQuery.SentSince(DateTime.UtcNow.AddMinutes(sentSinceMinutesFilter)));
            uids = await inbox.SearchAsync(uids, SearchQuery.SubjectContains(subjectFilter));
            uids = await inbox.SearchAsync(uids, SearchQuery.ToContains(recipientFilter));
            return uids;
        });

        await foreach (var result in results)
            yield return result;
    }
}
