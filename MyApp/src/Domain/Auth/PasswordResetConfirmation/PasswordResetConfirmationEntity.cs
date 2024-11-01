using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Domain.Auth.PasswordResetConfirmation;

public class PasswordResetConfirmationEntity : BaseConfirmationEntity<PasswordResetConfirmationEntity>
{
    public new static PasswordResetConfirmationEntity Create(int userId)
        => BaseConfirmationEntity<PasswordResetConfirmationEntity>.Create(userId);
}
