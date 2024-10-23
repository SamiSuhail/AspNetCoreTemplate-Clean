using System;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using MyApp.Utilities.Tasks;

namespace MyApp.Tests.System.Providers.Email;

public class EmailProvider(EmailSettings settings)
{
    public async IAsyncEnumerable<MimeMessage> GetEmails(
        Func<IMailFolder, Task<IEnumerable<UniqueId>>> filter,
        Func<EmailUsersSettings, EmailUser>? userSelector = null)
    {
        using var client = new ImapClient();

        await client.ConnectAsync(settings.Host, settings.Port, useSsl: false);

        var user = userSelector?.Invoke(settings.Users) ?? settings.Users.Default;
        await client.AuthenticateAsync(user.EmailAddress, user.Password);

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

public static class InboxFilterExtensions
{
    public static async Task<IEnumerable<UniqueId>> GetRecentEmailsWhereSubjectContains(
        this IMailFolder inbox,
        string subjectFilter,
        string recipientFilter,
        int sentSinceMinutesFilter = 5)
    {
        var uids = await inbox.SearchAsync(SearchQuery.SentSince(DateTime.UtcNow.AddMinutes(sentSinceMinutesFilter)));
        uids = await inbox.SearchIfAnyAsync(uids, SearchQuery.SubjectContains(subjectFilter));
        uids = await inbox.SearchIfAnyAsync(uids, SearchQuery.ToContains(recipientFilter));
        return uids;
    }

    public static async Task<IList<UniqueId>>  SearchIfAnyAsync(
        this IMailFolder inbox,
        IList<UniqueId> uids,
        SearchQuery query,
        CancellationToken cancellationToken = default)
        => uids.Count == 0
            ? []
            : await inbox.SearchAsync(uids, query, cancellationToken);
}
