namespace MyApp.Server.Infrastructure.Auth;

public class AuthSettings
{
    public const string SectionName = "Auth";
    public required JwtSettings Jwt { get; set; }

    public static AuthSettings Get(IConfiguration configuration)
        => configuration.GetSection(SectionName)
            .Get<AuthSettings>()!;
}

public class JwtSettings
{
    public required string PrivateKeyXml { get; set; }
    public required string PublicKeyXml { get; set; }
    public required int ExpirationHours { get; set; }
}
