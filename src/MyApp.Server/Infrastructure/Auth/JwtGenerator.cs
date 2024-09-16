using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MyApp.Server.Infrastructure.Auth;

public interface IJwtGenerator
{
    string CreateAccessToken(int userId, string username, string email);
    string CreateRefreshToken(int userId, string username, string email, int version);
}

public class JwtGenerator(AuthSettings authSettings, IClock clock) : IJwtGenerator
{
    public string CreateAccessToken(int userId, string username, string email)
    {
        List<Claim> claims =
            [
                new(JwtClaims.UserId, userId.ToString()),
                new(JwtClaims.Username, username),
                new(JwtClaims.Email, email),
            ];

        var expiration = clock.UtcNow.AddMinutes(authSettings.Jwt.AccessTokenExpirationMinutes);
        var jwt = CreateToken(authSettings.Jwt.PrivateKeyXml, claims, expiration);
        return jwt;
    }

    public string CreateRefreshToken(int userId, string username, string email, int version)
    {
        List<Claim> claims =
            [
                new(JwtClaims.UserId, userId.ToString()),
                new(JwtClaims.Username, username),
                new(JwtClaims.Email, email),
                new(JwtClaims.RefreshTokenVersion, version.ToString()),
            ];

        var expiration = clock.UtcNow.AddDays(authSettings.Jwt.RefreshTokenExpirationDays);
        var jwt = CreateToken(authSettings.Jwt.PrivateKeyXml, claims, expiration);
        return jwt;
    }

    private static string CreateToken(string privateKeyXml, List<Claim> claims, DateTime expiration)
    {
        var rsa = RSA.Create();
        rsa.FromXmlString(privateKeyXml);
        var key = new RsaSecurityKey(rsa);
        var cred = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiration,
            signingCredentials: cred);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}
