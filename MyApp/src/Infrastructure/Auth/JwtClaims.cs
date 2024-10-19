using System.IdentityModel.Tokens.Jwt;

namespace MyApp.Infrastructure.Auth;

public static class JwtClaims
{
    public const string UserId = JwtRegisteredClaimNames.Sub;
    public const string Username = JwtRegisteredClaimNames.UniqueName;
    public const string Email = JwtRegisteredClaimNames.Email;
    public const string RefreshTokenVersion = "token_version";
    public const string Scopes = "scopes";
}
