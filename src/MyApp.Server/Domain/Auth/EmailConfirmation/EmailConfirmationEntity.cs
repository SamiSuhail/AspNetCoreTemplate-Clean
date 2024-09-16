using MyApp.Server.Domain.Shared;

namespace MyApp.Server.Domain.Auth.EmailConfirmation;

public class EmailConfirmationEntity : BaseConfirmationEntity
{
    public static EmailConfirmationEntity Create(int userId)
    {
        var emailConfirmation = Create();
        emailConfirmation.UserId = userId;
        return emailConfirmation;
    }

    public static EmailConfirmationEntity Create()
    {
        return new()
        {
            Code = GenerateCode(),
            CreatedAt = DateTime.UtcNow,
        };
    }
}
