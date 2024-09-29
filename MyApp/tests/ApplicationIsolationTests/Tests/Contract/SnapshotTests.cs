namespace MyApp.ApplicationIsolationTests.Tests.Contract;

public class SnapshotTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task OpenApiSpec_MatchesSnapshot()
    {
        var client = AppFactory.CreateClient();
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        string openApiSpec = await response.Content.ReadAsStringAsync();

        await Verify(openApiSpec);
    }

    [Fact]
    public async Task GraphQLSchema_MatchesSnapshot()
    {
        var client = AppFactory.CreateClient();
        var response = await client.GetAsync("/graphql?sdl");
        string openApiSpec = await response.Content.ReadAsStringAsync();

        await Verify(openApiSpec);
    }
}
