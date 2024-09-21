using System.Reflection;

namespace MyApp.Presentation.Endpoints.Core;

public static class IEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, string pattern = "")
    {
        handler.ThrowIfAnonymousMethod();

        builder.MapGet(pattern, handler)
            .WithName(handler.Method.Name);

        return builder;
    }

    public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, string pattern = "")
    {
        handler.ThrowIfAnonymousMethod();

        builder.MapPost(pattern, handler)
            .WithName(handler.Method.Name);

        return builder;
    }

    public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, string pattern)
    {
        handler.ThrowIfAnonymousMethod();

        builder.MapPut(pattern, handler)
            .WithName(handler.Method.Name);

        return builder;
    }

    public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, string pattern)
    {
        handler.ThrowIfAnonymousMethod();

        builder.MapDelete(pattern, handler)
            .WithName(handler.Method.Name);

        return builder;
    }

    private static void ThrowIfAnonymousMethod(this Delegate input)
    {
        if (IsAnonymous(input.Method))
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");

        static bool IsAnonymous(MethodInfo method)
        {
            var invalidChars = new[] { '<', '>' };
            return method.Name.Any(invalidChars.Contains);
        }
    }
}