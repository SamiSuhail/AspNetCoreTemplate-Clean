namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public record AccessToken(int UserId, string Username, string Email, string[] Scopes);
public record RefreshToken(int UserId, string Username, string Email, int Version);

public interface IJwtReader
{
    AccessToken ReadAccessToken(string jwtBearer);
    RefreshToken? ReadRefreshToken(string jwtBearer);
}
