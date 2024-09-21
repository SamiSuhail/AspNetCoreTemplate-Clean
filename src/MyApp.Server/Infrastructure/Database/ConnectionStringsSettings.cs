using MyApp.Server.Infrastructure.Utilities;

namespace MyApp.Server.Infrastructure.Database;

public class ConnectionStringsSettings : BaseSettings<ConnectionStringsSettings>
{
    public required string Database { get; set; }
}
