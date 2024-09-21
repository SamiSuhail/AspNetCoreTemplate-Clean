namespace MyApp.Presentation.GraphQL;

public class Query
{
    public PingQuery Ping()
        => new();

    public MeQuery Me()
        => new();
}
