using MyApp.Server.Domain.Shared;

namespace MyApp.Server.Domain.Auth.UserConfirmation;

public class UserConfirmationEntity : BaseConfirmationEntity
{
    public static UserConfirmationEntity Create(int userId)
    {
        var userConfirmation = Create();
        userConfirmation.UserId = userId;
        return userConfirmation;
    }

    public static UserConfirmationEntity Create()
    {
        return new()
        {
            Code = GenerateCode(),
            CreatedAt = DateTime.UtcNow,
        };
    }
}
