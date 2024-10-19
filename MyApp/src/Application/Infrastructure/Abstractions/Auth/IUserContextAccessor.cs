namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public interface IUserContextAccessor
{
    AccessToken AccessToken { get; }
}
