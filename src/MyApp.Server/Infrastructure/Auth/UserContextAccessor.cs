using MyApp.Server.Infrastructure.Abstractions.Auth;
using MyApp.Server.Shared;

namespace MyApp.Server.Infrastructure.Auth;

public class UserContextAccessor : IUserContextAccessor
{
    const string BearerSchemaPrefix = "Bearer ";
    public UserContextAccessor(IHttpContextAccessor httpContextAccessor, IJwtReader jwtReader)
    {
        var jwtBearer = httpContextAccessor.HttpContext?.Request?.Headers?.Authorization.FirstOrDefault();

        if (jwtBearer.IsNullOrEmpty())
            throw new InvalidOperationException("No authorization header was found on the request.");

        var jwtBearerWithNoPrefix = jwtBearer![BearerSchemaPrefix.Length..];

        User = jwtReader.ReadAccessToken(jwtBearerWithNoPrefix);
    }

    public UserContext User { get; }
}
