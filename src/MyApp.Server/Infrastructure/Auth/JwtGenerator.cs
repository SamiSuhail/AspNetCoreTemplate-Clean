using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MyApp.Server.Infrastructure.Auth;

public interface IJwtGenerator
{
    string Create(int id, string username, string email);
}

public class JwtGenerator(AuthSettings authSettings) : IJwtGenerator
{
    public string Create(int id, string username, string email)
    {
        List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
            ];

        var rsa = RSA.Create();
        rsa.FromXmlString(authSettings.Jwt.PrivateKeyXml);
        var key = new RsaSecurityKey(rsa);
        var cred = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(authSettings.Jwt.ExpirationHours),
            signingCredentials: cred);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}
