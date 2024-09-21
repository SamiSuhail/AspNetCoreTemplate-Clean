using MyApp.Server.Infrastructure.Utilities;

namespace MyApp.Server.Infrastructure.Auth;

public class AuthSettings : BaseSettings<AuthSettings>
{
    public required JwtSettings Jwt { get; set; }
}

public class JwtSettings
{
    public required string PrivateKeyXml { get; set; }
    public required string PublicKeyXml { get; set; }
    public required int AccessTokenExpirationMinutes { get; set; }
    public required int RefreshTokenExpirationDays { get; set; }
}
