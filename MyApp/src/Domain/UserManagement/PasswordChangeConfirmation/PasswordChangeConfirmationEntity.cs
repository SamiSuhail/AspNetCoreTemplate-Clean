using MyApp.Domain.Auth.User;
using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Domain.UserManagement.PasswordChangeConfirmation;

public class PasswordChangeConfirmationEntity : BaseConfirmationEntity
{
    public string NewPasswordHash { get; private set; } = default!;

    public static PasswordChangeConfirmationEntity Create(int userId, string newPassword)
    {
        var entity = Create<PasswordChangeConfirmationEntity>(userId);
        entity.NewPasswordHash = newPassword.Hash();
        return entity;
    }
}