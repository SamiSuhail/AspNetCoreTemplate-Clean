using System.Reflection;

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
        await Verify(openApiSpec, sourceFile: GetSourceFile(nameof(OpenApiSpec_MatchesSnapshot)));
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
        await Verify(graphQlSchema, sourceFile: GetSourceFile(nameof(GraphQLSchema_MatchesSnapshot)));
    }

    private static string GetSourceFile(string testName)
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var parentDirectory = Directory.GetParent(assemblyLocation)!.FullName;
        return Path.Combine(parentDirectory, "Tests", "Contract", $"{nameof(SnapshotTests)}.{testName}.verified.txt");
    }
}
