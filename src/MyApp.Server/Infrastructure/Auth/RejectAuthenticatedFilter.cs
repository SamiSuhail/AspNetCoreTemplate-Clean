
namespace MyApp.Server.Infrastructure.Auth;

public class AnonymousOnlyFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var errors = new Dictionary<string, string[]>()
            {
                { "User", ["Authenticated users are not allowed."] },
            };
            return Results.ValidationProblem(errors);
        }

        return await next(context);
    }
}
