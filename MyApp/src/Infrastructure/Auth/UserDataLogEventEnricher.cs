using MyApp.Application.Infrastructure.Abstractions.Auth;
using Serilog.Core;

namespace MyApp.Infrastructure.Auth;

internal class UserDataLogEventEnricher(IUserContextAccessor userContextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var jwtBearer = userContextAccessor.JwtBearerOrDefault;
        if (jwtBearer == null)
            return;

        var userData = userContextAccessor.UserData;

        AddPropertyIfAbsent(nameof(userData.UserId), userData.UserId);
        AddPropertyIfAbsent(nameof(userData.Username), userData.Username);
        AddPropertyIfAbsent(nameof(jwtBearer), jwtBearer, maximumLogLevel: LogEventLevel.Verbose);
        AddPropertyIfAbsent(nameof(userData), userData, destructureObjects: true, maximumLogLevel: LogEventLevel.Verbose);

        void AddPropertyIfAbsent(string propertyName, object value, bool destructureObjects = false, LogEventLevel maximumLogLevel = LogEventLevel.Fatal)
        {
            // add check for global minimum log level too, if it is higher than the filter do not add to context
            if (logEvent.Level > maximumLogLevel)
                return;

            var property = propertyFactory.CreateProperty(propertyName, value, destructureObjects);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
