using MyApp.Server.Domain.Shared;

namespace MyApp.Server.Domain.Auth.PasswordResetConfirmation;

public class PasswordResetConfirmationEntity : BaseConfirmationEntity
{
    public static PasswordResetConfirmationEntity Create(int userId)
    {
        return new()
        {
            UserId = userId,
            Code = GenerateCode(),
            CreatedAt = DateTime.UtcNow,
        };
    }
}
