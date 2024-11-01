using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Domain.Auth.UserConfirmation;

public class UserConfirmationEntity : BaseConfirmationEntity<UserConfirmationEntity>
{
    public new static UserConfirmationEntity Create(int userId)
        => BaseConfirmationEntity<UserConfirmationEntity>.Create(userId);

    public new static UserConfirmationEntity Create()
        => BaseConfirmationEntity<UserConfirmationEntity>.Create();
}
