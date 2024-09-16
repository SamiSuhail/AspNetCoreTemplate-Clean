using MyApp.Server.Domain.Auth.User;
using static MyApp.Server.Domain.Shared.BaseConfirmationConstants;

namespace MyApp.Server.Domain.Shared;

public abstract class BaseConfirmationEntity
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
            Code = GenerateCode(),
            CreatedAt = DateTime.UtcNow,
        };
    }

    protected static string GenerateCode()
    {
        var codeMaxValueExclusive = (int)Math.Pow(10, CodeLength);
        return new Random().Next(codeMaxValueExclusive).ToString($"D{CodeLength}");
    }
}
