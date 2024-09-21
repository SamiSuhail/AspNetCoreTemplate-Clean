namespace MyApp.Server.Infrastructure.Abstractions.Auth;

public interface IUserContextAccessor
{
    UserContext User { get; }
}
