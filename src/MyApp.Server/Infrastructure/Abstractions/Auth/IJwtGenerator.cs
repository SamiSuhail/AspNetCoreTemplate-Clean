namespace MyApp.Server.Infrastructure.Abstractions.Auth;

public interface IJwtGenerator
{
    string CreateAccessToken(int userId, string username, string email);
    string CreateRefreshToken(int userId, string username, string email, int version);
}
