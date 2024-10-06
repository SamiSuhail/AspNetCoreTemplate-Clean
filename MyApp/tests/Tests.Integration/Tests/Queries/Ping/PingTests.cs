using MyApp.Application.Interfaces.Queries.Ping;
using MyApp.Tests.Utilities.Clients.Extensions;

namespace MyApp.Tests.Integration.Tests.Queries.Ping;

public class PingTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenUserIsUnauthorized_ReturnsResponse()
    {
        // Act
        var response = await UnauthorizedAppClient.Ping();

        // Assert
        AssertValidResponse(response);
    }

    [Fact]
    public async Task GivenUserIsAuthorized_ReturnsResponse()
    {
        // Act
        var response = await AppClient.Ping();

        // Assert
        AssertValidResponse(response);
    }

    private static void AssertValidResponse(IApiResponse<Pong> response)
    {
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        response.Content!.Message.Should().Be(Pong.DefaultMessage);
    }
}
