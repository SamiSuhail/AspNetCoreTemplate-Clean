using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;

namespace MyApp.Server.Domain.Auth.User;

public class UserEntity
{
    public int Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public bool IsEmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PasswordResetConfirmationEntity? PasswordResetConfirmation { get; private set; }
    public EmailConfirmationEntity? EmailConfirmation { get; private set; }

    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
    }

    public static UserEntity Create(string username, string password, string email)
        => new()
        {
            Username = username,
            PasswordHash = password.Hash(),
            Email = email,
            IsEmailConfirmed = false,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmation = EmailConfirmationEntity.Create(),
        };

    public void UpdatePassword(string password)
    {
        PasswordHash = password.Hash();
    }
}
