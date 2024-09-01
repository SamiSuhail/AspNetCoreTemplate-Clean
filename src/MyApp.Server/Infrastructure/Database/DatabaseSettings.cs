namespace MyApp.Server.Infrastructure.Database;

public class DatabaseSettings
{
    public const string SectionName = "Database";
    public int MaxRetryCount { get; set; }
    public int CommandTimeout { get; set; }
    public bool EnableDetailedErrors { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }

    public static DatabaseSettings Get(IConfiguration configuration)
        => configuration.GetSection(SectionName)
            .Get<DatabaseSettings>()!;
}
