using MyApp.ApplicationIsolationTests.Clients;
using MyApp.Server.Application.Queries.Ping;

namespace MyApp.ApplicationIsolationTests.Tests.Queries.Ping;

public class PingDatabaseTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenUserIsUnauthorized_ReturnsError()
    {
        // Act
        var response = await UnauthorizedAppClient.PingDatabase();

        // Assert
        response.AssertError(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenTokenHasExpired_ReturnsError()
    {
        // Arrange
        string accessToken = ScopedServices.ArrangeExpiredAccessToken();
        var client = AppFactory.CreateClientWithToken(accessToken);

        // Act
        var response = await client.PingDatabase();

        // Assert
        response.AssertError(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GivenUserIsAuthorized_ReturnsResponse()
    {
        // Act
        var response = await AppClient.PingDatabase();

        // Assert
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        response.Content!.Message.Should().Be(nameof(Pong));
    }
}
