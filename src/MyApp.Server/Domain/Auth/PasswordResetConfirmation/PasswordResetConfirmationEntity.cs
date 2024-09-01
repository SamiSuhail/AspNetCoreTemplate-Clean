using MyApp.Server.Domain.Auth.User;
using static MyApp.Server.Domain.Auth.PasswordResetConfirmation.PasswordResetConfirmationConstants;

namespace MyApp.Server.Domain.Auth.PasswordResetConfirmation;

public class PasswordResetConfirmationEntity
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Code { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public UserEntity User { get; private set; } = null!;

    public static PasswordResetConfirmationEntity Create(int userId)
    {
        var codeMaxValueExclusive = (int)Math.Pow(10, CodeLength);
        var code = new Random().Next(codeMaxValueExclusive).ToString($"D{CodeLength}");
        return new()
        {
            UserId = userId,
            Code = code,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
