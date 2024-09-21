using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Domain.Auth.PasswordResetConfirmation;

public class PasswordResetConfirmationEntity : BaseConfirmationEntity
{
    public static PasswordResetConfirmationEntity Create(int userId)
        => Create<PasswordResetConfirmationEntity>(userId);
}
