using MyApp.Utilities.Settings;

namespace MyApp.Tests.System.Providers.Email;

public class EmailSettings : BaseSettings<EmailSettings>
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required EmailAddressesSettings Addresses { get; set; }
    public EmailAuthSettings? Auth { get; set; }
}

public class EmailAuthSettings
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class EmailAddressesSettings
{
    public required string Default { get; set; }
    public required string Other { get; set; }
}