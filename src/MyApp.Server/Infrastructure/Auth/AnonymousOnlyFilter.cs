
namespace MyApp.Server.Infrastructure.Auth;

public static class AnonymousOnlyConstants
{
    public const string Key = "User";
    public const string Message = "Authenticated users are not allowed.";
}

public class AnonymousOnlyFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var errors = new Dictionary<string, string[]>()
            {
                { AnonymousOnlyConstants.Key, [AnonymousOnlyConstants.Message] },
            };
            return Results.ValidationProblem(errors);
        }

        return await next(context);
    }
}
