namespace MyApp.Server.Infrastructure.Database;

public class ConnectionStringsSettings
{
    public const string SectionName = "ConnectionStrings";
    public required string Database { get; set; }

    public static ConnectionStringsSettings Get(IConfiguration configuration)
        => configuration.GetSection(SectionName)
            .Get<ConnectionStringsSettings>()!;
}
