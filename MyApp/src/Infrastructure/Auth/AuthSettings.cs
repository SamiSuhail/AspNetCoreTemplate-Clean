using MyApp.Utilities.Settings;

namespace MyApp.Infrastructure.Auth;

public class AuthSettings : BaseSettings<AuthSettings>
{
    public required JwtSettings Jwt { get; set; }
    public required AdminUserSettings AdminUser { get; set; }
}

public class JwtSettings
{
    public required string PrivateKeyXml { get; set; }
    public required string PublicKeyXml { get; set; }
    public required int AccessTokenExpirationMinutes { get; set; }
    public required int RefreshTokenExpirationDays { get; set; }
}

public class AdminUserSettings
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}
