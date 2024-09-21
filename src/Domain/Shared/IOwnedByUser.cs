using MyApp.Domain.Auth.User;

namespace MyApp.Domain.Shared;

public interface IOwnedByUser
{
    public int UserId { get; }

    public UserEntity User { get; }
}
