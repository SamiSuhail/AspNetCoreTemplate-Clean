using MyApp.Domain.Auth.User;
using MyApp.Domain.Shared;

namespace MyApp.Domain.Infra.Instance;

public class InstanceEntity : ICreationAudited
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public bool IsCleanupEnabled { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<UserEntity> Users { get; private set; } = default!;

    public static InstanceEntity Create(string name, bool isCleanupEnabled = false)
    {
        return new()
        {
            Name = name,
            IsCleanupEnabled = isCleanupEnabled,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
