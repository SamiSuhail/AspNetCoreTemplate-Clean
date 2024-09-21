using MyApp.Infrastructure.Utilities;

namespace MyApp.Infrastructure.Database;

public class ConnectionStringsSettings : BaseSettings<ConnectionStringsSettings>
{
    public required string Database { get; set; }
}
