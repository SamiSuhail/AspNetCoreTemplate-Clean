using MyApp.Utilities.Settings;

namespace MyApp.Tests.System.Core.Settings;

public class ServerSettings : BaseSettings<ServerSettings>
{
    public required string BaseUrl { get; set; }
    public required AuthSettings Auth { get; set; }
}

public class AuthSettings
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}