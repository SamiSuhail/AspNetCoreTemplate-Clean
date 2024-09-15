using MyApp.Server.Domain;

namespace MyApp.Server.Presentation.Startup.Middleware;

public class CustomGraphQLExceptionHandler : IErrorFilter
{
    private readonly Dictionary<Type, Func<IError, IError>> _exceptionHandlers;

    public CustomGraphQLExceptionHandler()
    {
        _exceptionHandlers = new()
        {
            [typeof(DomainException)] = HandleDomainException
        };
    }

    public IError OnError(IError error)
    {
        var exception = error.Exception;
        if (exception == null)
            return error;

        var exceptionType = exception.GetType();

        if (!_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            return error;
        }

        return handler.Invoke(error);
    }

    private AggregateError HandleDomainException(IError error)
    {
        var exception = (DomainException)error.Exception!;
        var cleanedError = GetCleanedError(error);

        var errors = exception.Failure.Errors.Select(e =>
                ErrorBuilder.FromError(cleanedError)
                    .SetCode(e.Key)
                    .SetMessage(e.Message)
                    .Build())
            .ToArray();

        return new AggregateError(errors);
    }

    private static IError GetCleanedError(IError error)
    {
        var errorBuilder = ErrorBuilder.FromError(error)
                    .RemoveException();

        foreach (var key in error.Extensions?.Keys ?? [])
        {
            errorBuilder.RemoveExtension(key);
        }

        return errorBuilder.Build();
    }
}
