using MyApp.Server.Domain.Auth.User;

namespace MyApp.Server.Domain.Shared;

public interface IOwnedByUser
{
    public int UserId { get; }

    public UserEntity User { get; }
}
