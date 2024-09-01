using MyApp.Server.Domain.Auth.User;
using static MyApp.Server.Domain.Auth.EmailConfirmation.EmailConfirmationConstants;

namespace MyApp.Server.Domain.Auth.EmailConfirmation;

public class EmailConfirmationEntity
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Code { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public UserEntity User { get; private set; } = null!;

    public static EmailConfirmationEntity Create(int userId)
    {
        var emailConfirmation = Create();
        emailConfirmation.UserId = userId;
        return emailConfirmation;
    }

    public static EmailConfirmationEntity Create()
    {
        var codeMaxValueExclusive = (int)Math.Pow(10, CodeLength);
        var code = new Random().Next(codeMaxValueExclusive).ToString($"D{CodeLength}");
        return new()
        {
            Code = code,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
