namespace MyApp.Presentation.GraphQL;

public class Query
{
    public Ping Ping()
        => new();

    public Me Me()
        => new();
}
