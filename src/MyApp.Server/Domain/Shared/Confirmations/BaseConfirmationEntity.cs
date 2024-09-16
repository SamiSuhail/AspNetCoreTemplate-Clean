using MyApp.Server.Domain.Auth.User;

namespace MyApp.Server.Domain.Shared.Confirmations;

public abstract class BaseConfirmationEntity : ICreationAudited, IOwnedByUser
{
    public int Id { get; protected set; }
    public int UserId { get; protected set; }
    public string Code { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }

    public UserEntity User { get; protected set; } = default!;

    protected static TEntity Create<TEntity>(int userId) where TEntity : BaseConfirmationEntity, new()
    {
        var confirmation = Create<TEntity>();
        confirmation.UserId = userId;
        return confirmation;
    }

    protected static TEntity Create<TEntity>() where TEntity : BaseConfirmationEntity, new()
    {
        return new()
        {
            Code = ConfirmationCodeGenerator.GenerateCode(),
            CreatedAt = DateTime.UtcNow,
        };
    }
}
