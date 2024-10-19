using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.EmailChangeConfirmation;
using MyApp.Domain.Shared;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation;
using MyApp.Domain.Infra.Instance;
using MyApp.Domain.Access.Scope;

namespace MyApp.Domain.Auth.User;

public class UserEntity : ICreationAudited
{
    public int Id { get; private set; }
    public int InstanceId { get; private set; }
    public string Username { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public bool IsEmailConfirmed { get; private set; }
    public int RefreshTokenVersion { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public InstanceEntity Instance { get; private set; } = default!;
    public PasswordResetConfirmationEntity? PasswordResetConfirmation { get; private set; }
    public UserConfirmationEntity? UserConfirmation { get; private set; }
    public EmailChangeConfirmationEntity? EmailChangeConfirmation { get; private set; }
    public PasswordChangeConfirmationEntity? PasswordChangeConfirmation { get; private set; }
    public ICollection<UserScopeEntity> UserScopes { get; private set; } = default!;

    public static UserEntity CreateConfirmed(int instanceId, string username, string password, string email)
        => new()
        {
            InstanceId = instanceId,
            Username = username,
            PasswordHash = password.Hash(),
            Email = email,
            IsEmailConfirmed = true,
            RefreshTokenVersion = 1,
            CreatedAt = DateTime.UtcNow,
        };

    public static UserEntity Create(int instanceId, string username, string password, string email)
    {
        var user = CreateConfirmed(instanceId, username, password, email);
        user.IsEmailConfirmed = false;
        user.UserConfirmation = UserConfirmationEntity.Create();
        return user;
    }

    public void AddScopes(ScopeEntity[] scopes)
    {
        foreach (var scope in scopes)
        {
            var userScope = UserScopeEntity.Create(Id, scope);
            UserScopes.Add(userScope);
        }
    }

    public void ConfirmUserRegistration()
    {
        IsEmailConfirmed = true;
    }

    public void UpdatePassword(string password)
    {
        PasswordHash = password.Hash();
    }

    public void ConfirmPasswordChange(string passwordHash)
    {
        PasswordHash = passwordHash;
        SignOutOnAllDevices();
    }

    public void ConfirmEmailChange()
    {
        Email = EmailChangeConfirmation!.NewEmail;
        SignOutOnAllDevices();
    }

    public void SignOutOnAllDevices()
    {
        RefreshTokenVersion++;
    }
}
