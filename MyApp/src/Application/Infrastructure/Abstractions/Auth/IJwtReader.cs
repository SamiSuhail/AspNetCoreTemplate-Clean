namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public record AccessToken(int Id, string Username, string Email);
public record RefreshToken(int UserId, string Username, string Email, int Version);

public interface IJwtReader
{
    AccessToken ReadAccessToken(string jwtBearer);
    RefreshToken? ReadRefreshToken(string jwtBearer);
}
