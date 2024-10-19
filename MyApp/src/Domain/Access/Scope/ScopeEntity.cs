using FluentAssertions;
using MyApp.Domain.Shared;

namespace MyApp.Domain.Access.Scope;

public class ScopeEntity : ICreationAudited
{
    public int Id { get; set; }
    public string Name { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    public ICollection<UserScopeEntity> ScopeUsers { get; private set; } = default!;

    public static ScopeEntity Create(string name)
    {
        name.AsEnumerable().Should().NotContain(ScopeCollection.Separator);

        return new()
        {
            Name = name,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
