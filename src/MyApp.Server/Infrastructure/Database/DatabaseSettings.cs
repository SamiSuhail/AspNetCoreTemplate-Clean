using MyApp.Server.Infrastructure.Utilities;

namespace MyApp.Server.Infrastructure.Database;

public class DatabaseSettings : BaseSettings<DatabaseSettings>
{
    public int MaxRetryCount { get; set; }
    public int CommandTimeout { get; set; }
    public bool EnableDetailedErrors { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
}
