using MyApp.Infrastructure.Utilities;

namespace MyApp.Infrastructure.Database;

public class DatabaseSettings : BaseSettings<DatabaseSettings>
{
    public int MaxRetryCount { get; set; }
    public int CommandTimeout { get; set; }
    public bool EnableDetailedErrors { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
}
