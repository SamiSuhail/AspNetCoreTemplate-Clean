using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MyApp.Infrastructure.Auth;

public static class JwtValidationParametersProvider
{
    public static TokenValidationParameters Get(string publicKeyXml)
    {
        var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };
        return tokenValidationParameters;
    }
}
