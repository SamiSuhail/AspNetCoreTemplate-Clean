using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Infrastructure.Abstractions.Logging;
using MyApp.Domain;

namespace MyApp.Presentation.Startup.Middleware;

public static class UnhandledExceptionConstants
{
    public const string Message = "An unexpected error occurred - please try again later. If the error persists, please contact support.";
}

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, CancellationToken, Task>> _exceptionHandlers;
    private readonly ILogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;
        _exceptionHandlers = new()
        {
            [typeof(DomainException)] = HandleDomainException
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            await handler.Invoke(httpContext, exception, cancellationToken);
            return true;
        }

        _logger.Error(exception, "An unhandled exception has occurred!");
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsync(UnhandledExceptionConstants.Message, cancellationToken);
        return true;
    }

    private async Task HandleDomainException(HttpContext httpContext, Exception ex, CancellationToken cancellationToken)
    {
        var exception = (DomainException)ex;

        _logger.ForContext(nameof(exception.Failure.Errors), exception.Failure.Errors, destructureObjects: true)
            .Warning(ex, "A domain exception has occurred.");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errors = AsErrors(exception.Failure.Errors);
        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(errors)
        {
            Title = "One or more errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        }, cancellationToken);

        static Dictionary<string, string[]> AsErrors(IEnumerable<DomainError> errors)
            => errors.GroupBy(e => e.Key, e => e.Message)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
