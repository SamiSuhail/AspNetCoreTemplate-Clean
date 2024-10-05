using MyApp.Utilities.Settings;

namespace MyApp.Infrastructure.Database;

public class ConnectionStringsSettings : BaseSettings<ConnectionStringsSettings>
{
    public required string Database { get; set; }
}
