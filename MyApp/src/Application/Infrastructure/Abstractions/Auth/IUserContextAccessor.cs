namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public interface IUserContextAccessor
{
    string JwtBearer { get; }
    string? JwtBearerOrDefault { get; }
    AccessTokenData UserData { get; }
    AccessTokenData? UserDataOrDefault { get; }
}
