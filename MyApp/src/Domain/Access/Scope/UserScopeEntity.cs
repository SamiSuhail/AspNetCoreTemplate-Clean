using MyApp.Domain.Auth.User;
using MyApp.Domain.Shared;

namespace MyApp.Domain.Access.Scope;

public class UserScopeEntity : ICreationAudited
{
    public int UserId { get; private set; }
    public int ScopeId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public UserEntity User { get; private set; } = default!;
    public ScopeEntity Scope { get; private set; } = default!;

    public static UserScopeEntity Create(int userId, int scopeId)
        => new()
        {
            UserId = userId,
            ScopeId = scopeId,
            CreatedAt = DateTime.UtcNow,
        };

    public static UserScopeEntity Create(int userId, ScopeEntity scope)
        => new()
        {
            UserId = userId,
            Scope = scope,
            CreatedAt = DateTime.UtcNow,
        };
}