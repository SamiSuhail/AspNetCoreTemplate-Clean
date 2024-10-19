using MyApp.Application.Infrastructure.Abstractions.Auth;

namespace MyApp.Infrastructure.Http;

public class HttpUserContextAccessor(IHttpContextAccessor httpContextAccessor, IJwtReader jwtReader) : IUserContextAccessor
{
    const string BearerSchemaPrefix = "Bearer ";
    private AccessToken? _accessToken;

    public AccessToken AccessToken => _accessToken ??= ReadAccessToken();

    private AccessToken ReadAccessToken()
    {
        var headers = httpContextAccessor.HttpContext?.Request?.Headers
                ?? throw new InvalidOperationException("No request headers accessible in scope.");

        var jwtBearer = headers.Authorization.FirstOrDefault();
            if (jwtBearer is null or [])
                throw new InvalidOperationException("No authorization header found.");

        var jwtBearerWithNoPrefix = jwtBearer![BearerSchemaPrefix.Length..];
        return jwtReader.ReadAccessToken(jwtBearerWithNoPrefix);
    }
}
