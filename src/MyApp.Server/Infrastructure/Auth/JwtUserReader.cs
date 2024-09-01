using System.Security.Claims;

namespace MyApp.Server.Infrastructure.Auth;

public record JwtUserData(int Id, string Username, string Email);

public interface IJwtUserReader
{
    JwtUserData User { get; }
}

public class JwtUserReader : IJwtUserReader
{
    public JwtUserReader(IHttpContextAccessor httpContextAccessor)
    {
        var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity
            ?? throw new InvalidOperationException("No identity was available in the HTTP context.");

        User = new(
            Id: int.Parse(GetClaim(identity, ClaimTypes.NameIdentifier)),
            Username: GetClaim(identity, ClaimTypes.Name),
            Email: GetClaim(identity, ClaimTypes.Email)
            );
    }

    public JwtUserData User { get; }

    private static string GetClaim(ClaimsIdentity identity, string type)
    {
        var claim = identity.FindFirst(type)?.Value;
        ArgumentException.ThrowIfNullOrWhiteSpace(claim, nameof(claim));
        return claim;
    }
}
