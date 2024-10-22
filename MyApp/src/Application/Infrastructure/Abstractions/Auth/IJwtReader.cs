namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public record AccessTokenData(int UserId, string Username, string Email, string[] Scopes);
public record RefreshTokenData(int UserId, int Version);

public interface IJwtReader
{
    AccessTokenData? ReadAccessToken(string jwtBearer);
    RefreshTokenData? ReadRefreshToken(string jwtBearer);
}
