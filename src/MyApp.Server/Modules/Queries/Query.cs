using MyApp.Server.Modules.Queries.Ping;

namespace MyApp.Server.Modules.Queries;

public class Query
{
    public PingQuery Ping()
        => new();

    public MeQuery Me()
        => new();
}
