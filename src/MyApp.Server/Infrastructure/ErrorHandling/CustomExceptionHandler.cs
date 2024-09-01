using MyApp.Server.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Server.Infrastructure.ErrorHandling;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;
    private readonly ILogger _logger;

    public CustomExceptionHandler(ILogger logger)
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
            await handler.Invoke(httpContext, exception);
            return true;
        }

        _logger.Error(exception, "An unhandled exception has occured!");
        return false;
    }

    private async Task HandleDomainException(HttpContext httpContext, Exception ex)
    {
        var exception = (DomainException)ex;

        _logger.ForContext(nameof(exception.Failure.Errors), exception.Failure.Errors, destructureObjects: true)
            .Warning(ex, "A domain exception has occured.");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errors = AsErrors(exception.Failure.Errors);
        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(errors)
        {
            Title = "One or more errors occured.",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });

        static Dictionary<string, string[]> AsErrors(IEnumerable<DomainError> errors)
            => errors.GroupBy(e => e.Key, e => e.Message)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
