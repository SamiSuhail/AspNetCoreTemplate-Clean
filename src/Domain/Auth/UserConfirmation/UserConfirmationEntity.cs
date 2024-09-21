using MyApp.Domain.Shared.Confirmations;

namespace MyApp.Domain.Auth.UserConfirmation;

public class UserConfirmationEntity : BaseConfirmationEntity
{
    public static UserConfirmationEntity Create(int userId)
        => Create<UserConfirmationEntity>(userId);

    public static UserConfirmationEntity Create()
        => Create<UserConfirmationEntity>();
}
