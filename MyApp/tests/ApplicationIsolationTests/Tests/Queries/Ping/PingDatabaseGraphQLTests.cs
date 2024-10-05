using MyApp.Application.Interfaces.Queries.Ping;

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
        var accessToken = ScopedServices.ArrangeExpiredAccessToken(User);
        var graphQlClient = AppFactory.ArrangeGraphQLClientWithToken(accessToken);

        // Act
        var response = await graphQlClient.PingDatabase.ExecuteAsync();

        // Assert
        response.AssertUnauthorized();
    }

    [Fact]
    public async Task GivenTokenIsInvalid_ReturnsError()
    {
        // Arrange
        var jwtGenerator = ScopedServices.ArrangeJwtGeneratorWithInvalidPrivateKey();
        var accessToken = jwtGenerator.CreateAccessToken(User.Entity.Id, User.Entity.Username, User.Entity.Email);
        var graphQlClient = AppFactory.ArrangeGraphQLClientWithToken(accessToken);

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
        response.Data.Ping.Database.Message.Should().Be(Pong.DefaultMessage);
    }
}
