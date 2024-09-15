using MyApp.Server.Modules.Queries.Ping;

namespace MyApp.ApplicationIsolationTests.Tests.Queries.Ping;

public class PingGraphQLTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenUserIsUnauthorized_ReturnsResponse()
    {
        // Act
        var response = await UnauthorizedGraphQLClient.Ping.ExecuteAsync();

        // Assert
        AssertValidResponse(response);
    }

    [Fact]
    public async Task GivenUserIsAuthorized_ReturnsResponse()
    {
        // Act
        var response = await GraphQLClient.Ping.ExecuteAsync();

        // Assert
        AssertValidResponse(response);
    }

    private static void AssertValidResponse(IOperationResult<IPingResult> response)
    {
        response.AssertSuccess();
        response.Data!.Ping.Should().NotBeNull();
        response.Data.Ping.Default.Should().NotBeNull();
        response.Data.Ping.Default.Message.Should().Be(nameof(Pong));
    }
}
