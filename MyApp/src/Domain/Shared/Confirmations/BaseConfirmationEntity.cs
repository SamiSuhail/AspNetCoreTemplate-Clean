using MyApp.Domain.Auth.User;

namespace MyApp.Domain.Shared.Confirmations;

public abstract class BaseConfirmationEntity<TEntity> : ICreationAudited, IOwnedByUser 
    where TEntity : BaseConfirmationEntity<TEntity>, new()
{
    public int Id { get; protected set; }
    public int UserId { get; protected set; }
    public string Code { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }

    public UserEntity User { get; protected set; } = default!;

    protected static TEntity Create(int userId)
    {
        var confirmation = Create();
        confirmation.UserId = userId;
        return confirmation;
    }

    protected static TEntity Create()
    {
        return new()
        {
            Code = ConfirmationCodeGenerator.GenerateCode(),
            CreatedAt = DateTime.UtcNow,
        };
    }
}
