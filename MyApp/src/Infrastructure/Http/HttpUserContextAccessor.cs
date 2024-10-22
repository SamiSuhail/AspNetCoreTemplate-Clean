using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Infrastructure.Logging;

namespace MyApp.Infrastructure.Http;

public class HttpUserContextAccessor(
    IHttpContextAccessor httpContextAccessor,
    IJwtReader jwtReader
    ) : IUserContextAccessor
{
    const string BearerSchemaPrefix = "Bearer ";
    private readonly ILogger<HttpUserContextAccessor> _logger = new Logger<HttpUserContextAccessor>(Log.Logger);

    public string? JwtBearerOrDefault => ReadJwtBearer();
    public AccessTokenData? UserDataOrDefault => ReadAccessToken();
    public string JwtBearer 
        => JwtBearerOrDefault ?? throw new InvalidOperationException("No user data available.");
    public AccessTokenData UserData
        => UserDataOrDefault ?? throw new InvalidOperationException("No user data available.");

    private string? ReadJwtBearer()
    {
        var headers = httpContextAccessor.HttpContext?.Request?.Headers;

        if (headers == null)
        {
            _logger.Debug("No request headers accessible in scope.");
            return null;
        }

        var jwtBearer = headers.Authorization.FirstOrDefault();
        if (jwtBearer is null or [])
        {
            _logger.Debug("No authorization header found.");
            return null;
        }

        return jwtBearer;
    }

    private AccessTokenData? ReadAccessToken()
    {
        if (JwtBearerOrDefault == null)
            return null;

        var jwtBearerWithNoPrefix = JwtBearerOrDefault[BearerSchemaPrefix.Length..];
        return jwtReader.ReadAccessToken(jwtBearerWithNoPrefix);
    }
}
