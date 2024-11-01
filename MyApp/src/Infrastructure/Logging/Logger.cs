namespace MyApp.Infrastructure.Logging;

public class Logger<T>(ILogger logger) : ILogger<T>
{
    private readonly ILogger _inner = logger.ForContext<T>();
    public void Write(LogEvent logEvent)
        => _inner.Write(logEvent);
}
