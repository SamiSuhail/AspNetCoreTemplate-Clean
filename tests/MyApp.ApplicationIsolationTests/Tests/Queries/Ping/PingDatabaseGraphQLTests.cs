using MyApp.ApplicationIsolationTests.Clients;
using MyApp.Server.Application.Queries.Ping;

namespace MyApp.ApplicationIsolationTests.Tests.Queries.Ping;

public class PingDatabaseGraphQLTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenUserIsUnauthorized_ReturnsError()
    {
        // Act
        var response = await UnauthorizedGraphQLClient.PingDatabase.ExecuteAsync();

        // Assert
        response.AssertUnauthorized();
    }

    [Fact]
    public async Task GivenTokenHasExpired_ReturnsError()
    {
        // Arrange
        string accessToken = ScopedServices.ArrangeExpiredAccessToken();
        var graphQlClient = AppFactory.CreateGraphQLClientWithToken(accessToken);

        // Act
        var response = await graphQlClient.PingDatabase.ExecuteAsync();

        // Assert
        response.AssertUnauthorized();
    }

    [Fact]
    public async Task GivenUserIsAuthorized_ReturnsResponse()
    {
        // Act
        var response = await GraphQLClient.PingDatabase.ExecuteAsync();

        // Assert
        response.AssertSuccess();
        response.Data!.Ping.Should().NotBeNull();
        response.Data.Ping.Database.Should().NotBeNull();
        response.Data.Ping.Database.Message.Should().Be(nameof(Pong));
    }
}
