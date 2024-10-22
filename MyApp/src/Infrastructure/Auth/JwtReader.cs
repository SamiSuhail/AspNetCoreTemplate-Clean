using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Infrastructure.Logging;

namespace MyApp.Infrastructure.Auth;

public class JwtReader(AuthSettings settings) : IJwtReader
{
    private readonly Logger<JwtReader> _logger = new(Log.Logger);
    public AccessTokenData? ReadAccessToken(string jwtBearer)
    {
        var (identity, ex) = ReadTokenClaims(settings.Jwt.PublicKeyXml, jwtBearer);

        if (identity == null)
        {
            _logger.ForContext(LogEventLevel.Verbose, nameof(jwtBearer), jwtBearer)
                .Debug(ex, "No identity could be generated with the provided access token.");
            return null;
        }

        return new(
            UserId: int.Parse(GetClaim(identity, JwtClaims.UserId)),
            Username: GetClaim(identity, JwtClaims.Username),
            Email: GetClaim(identity, JwtClaims.Email),
            Scopes: GetClaims(identity, JwtClaims.Scopes)
            );
    }

    public RefreshTokenData? ReadRefreshToken(string jwtBearer)
    {
        var (identity, ex) = ReadTokenClaims(settings.Jwt.PublicKeyXml, jwtBearer);

        if (identity == null)
        {
            _logger.ForContext(LogEventLevel.Verbose, nameof(jwtBearer), jwtBearer)
                .Debug(ex, "No identity could be generated with the provided refresh token.");
            return null;
        }

        return new(
            UserId: int.Parse(GetClaim(identity, JwtClaims.UserId)),
            Version: int.Parse(GetClaim(identity, JwtClaims.RefreshTokenVersion))
            );
    }

    private static (ClaimsIdentity?, Exception?) ReadTokenClaims(string publicKeyXml, string jwtBearer)
    {
        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        var validationParameters = JwtValidationParametersProvider.Get(publicKeyXml);
        try
        {
            var principal = tokenHandler.ValidateToken(jwtBearer, validationParameters, out var _);
            return (principal.Identity as ClaimsIdentity, null);
        }
        catch (Exception ex)
        {
            return (null, ex);
        }
    }

    private static string GetClaim(ClaimsIdentity identity, string type)
    {
        var claim = GetClaimOrDefault(identity, type);
        claim.Should().NotBeNull(because: $"Claim {type} is required.");

        return claim!;
    }

    private static string? GetClaimOrDefault(ClaimsIdentity identity, string type, string? defaultValue = null)
        => GetClaims(identity, type).FirstOrDefault() ?? defaultValue;

    private static string[] GetClaims(ClaimsIdentity identity, string type)
    {
        var claims = identity.FindAll(type)
            .Select(c => c.Value)
            .ToArray();

        return claims;
    }
}
