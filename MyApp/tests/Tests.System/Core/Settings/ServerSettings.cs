using MyApp.Utilities.Settings;

namespace MyApp.Tests.System.Core.Settings;

public class ServerSettings : BaseSettings<ServerSettings>
{
    public required string BaseUrl { get; set; }
    public required ServerAdminAuthSettings AdminAuth { get; set; }
}

public class ServerAdminAuthSettings
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}