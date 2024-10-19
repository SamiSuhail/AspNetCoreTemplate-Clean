using MyApp.Domain.Access.Scope;

namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public interface IJwtGenerator
{
    string CreateAccessToken(int userId, string username, string email, ScopeCollection scopes);
    string CreateRefreshToken(int userId, string username, string email, int version);
}
