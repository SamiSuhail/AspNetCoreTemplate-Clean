using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyApp.Server.Infrastructure.Auth;

public record UserContext(int Id, string Username, string Email);

public interface IJwtReader
{
    UserContext ReadAccessToken(string jwtBearer);
}

public class JwtReader(AuthSettings settings) : IJwtReader
{
    public UserContext ReadAccessToken(string jwtBearer)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = JwtValidationParametersProvider.Get(settings.Jwt.PublicKeyXml);
        var principal = tokenHandler.ValidateToken(jwtBearer, validationParameters, out var _);
        var identity = principal.Identity as ClaimsIdentity
            ?? throw new InvalidOperationException("No identity could be generated with the provided access token.");

        return new(
            Id: int.Parse(GetClaim(identity, ClaimTypes.NameIdentifier)),
            Username: GetClaim(identity, ClaimTypes.Name),
            Email: GetClaim(identity, ClaimTypes.Email)
            );
    }

    private static string GetClaim(ClaimsIdentity identity, string type)
    {
        var claim = identity.FindFirst(type)?.Value;
        ArgumentException.ThrowIfNullOrWhiteSpace(claim, nameof(claim));
        return claim;
    }
}
