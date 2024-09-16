using MyApp.Server.Domain.Auth.UserConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;

namespace MyApp.Server.Domain.Auth.User;

public class UserEntity
{
    public int Id { get; private set; }
    public string Username { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public bool IsEmailConfirmed { get; private set; }
    public int RefreshTokenVersion { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PasswordResetConfirmationEntity? PasswordResetConfirmation { get; private set; }
    public UserConfirmationEntity? UserConfirmation { get; private set; }

    public void ConfirmUserRegistration()
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
            RefreshTokenVersion = 1,
            CreatedAt = DateTime.UtcNow,
            UserConfirmation = UserConfirmationEntity.Create(),
        };

    public void SignOutOnAllDevices()
    {
        RefreshTokenVersion++;
    }

    public void UpdatePassword(string password)
    {
        PasswordHash = password.Hash();
    }
}
