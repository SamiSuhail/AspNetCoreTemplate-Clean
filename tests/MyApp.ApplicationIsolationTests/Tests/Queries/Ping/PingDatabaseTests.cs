using MyApp.Server.Modules.Queries.Ping;

namespace MyApp.ApplicationIsolationTests.Tests.Queries.Ping;

public class PingDatabaseTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenUserIsUnauthorized_ThenReturnsError()
    {
        // Act
        var response = await UnauthorizedAppClient.PingDatabase();

        // Assert
        response.AssertError(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUserIsAuthorized_ThenReturnsResponse()
    {
        // Act
        var response = await AppClient.PingDatabase();

        // Assert
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        response.Content!.Message.Should().Be(nameof(Pong));
    }
}
