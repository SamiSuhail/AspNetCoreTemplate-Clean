namespace MyApp.Tests.Integration.Tests.Contract;

public class SnapshotTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task OpenApiSpec_MatchesSnapshot()
    {
        // Arrange
        var client = AppFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        string openApiSpec = await response.Content.ReadAsStringAsync();
        await Verify(openApiSpec);
    }

    [Fact]
    public async Task GraphQLSchema_MatchesSnapshot()
    {
        // Arrange
        var client = AppFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/graphql?sdl");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        string graphQlSchema = await response.Content.ReadAsStringAsync();
        await Verify(graphQlSchema);
    }
}
