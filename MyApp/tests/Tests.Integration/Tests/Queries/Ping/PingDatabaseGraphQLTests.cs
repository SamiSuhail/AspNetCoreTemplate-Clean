using MyApp.Application.Interfaces.Queries.Ping;

namespace MyApp.Tests.Integration.Tests.Queries.Ping;

public class PingDatabaseGraphQLTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ReturnsResponse()
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
