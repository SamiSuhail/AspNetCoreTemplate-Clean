using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Utilities.Strings;

namespace MyApp.Infrastructure.RequestContext;

public class RequestContextAccessor : IRequestContextAccessor
{
    const string BearerSchemaPrefix = "Bearer ";
    private readonly IJwtReader _jwtReader;

    public RequestContextAccessor(IHttpContextAccessor httpContextAccessor, IJwtReader jwtReader)
    {
        _jwtReader = jwtReader;
        var headers = httpContextAccessor.HttpContext?.Request?.Headers
            ?? throw new InvalidOperationException("No request headers accessible in scope.");
        AccessToken = ReadAccessToken(headers);
    }

    public AccessToken AccessToken { get; }

    private AccessToken ReadAccessToken(IHeaderDictionary headers)
    {
        var jwtBearer = headers.Authorization.FirstOrDefault();
        if (jwtBearer.IsNullOrEmpty())
            throw new InvalidOperationException("No authorization header found.");

        var jwtBearerWithNoPrefix = jwtBearer![BearerSchemaPrefix.Length..];
        return _jwtReader.ReadAccessToken(jwtBearerWithNoPrefix);
    }
}
