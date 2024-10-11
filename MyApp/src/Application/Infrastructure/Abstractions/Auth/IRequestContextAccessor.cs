namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public interface IRequestContextAccessor
{
    AccessToken AccessToken { get; }
}
