namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public interface IUserContextAccessor
{
    UserContext User { get; }
}
