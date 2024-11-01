using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Domain.Access.Scope;

namespace MyApp.Infrastructure.Auth;

public class JwtGenerator(AuthSettings authSettings, IClock clock) : IJwtGenerator
{
    public string CreateAccessToken(int userId, string username, string email, ScopeCollection scopes)
    {
        List<Claim> claims =
            [
                new(JwtClaims.UserId, userId.ToString()),
                new(JwtClaims.Username, username),
                new(JwtClaims.Email, email),
            ];

        claims.AddRange(scopes.Collection.Select(scope => new Claim(JwtClaims.Scopes, scope)));

        var expiration = clock.UtcNow.AddMinutes(authSettings.Jwt.AccessTokenExpirationMinutes);
        var jwt = CreateToken(authSettings.Jwt.PrivateKeyXml, claims, expiration);
        return jwt;
    }

    public string CreateRefreshToken(int userId, int version)
    {
        List<Claim> claims =
            [
                new(JwtClaims.UserId, userId.ToString()),
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
