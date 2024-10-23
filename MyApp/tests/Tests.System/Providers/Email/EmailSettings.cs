using MyApp.Utilities.Settings;

namespace MyApp.Tests.System.Providers.Email;

public class EmailSettings : BaseSettings<EmailSettings>
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required EmailUsersSettings Users { get; set; }
}

public class EmailUsersSettings
{
    public required EmailUser Default { get; set; }
    public required EmailUser Other { get; set; }
}

public class EmailUser
{
    public required string EmailAddress { get; set; }
    public required string Password { get; set; }
}