﻿using MyApp.Server.Application.Queries.Ping;

namespace MyApp.ApplicationIsolationTests.Tests.Queries.Ping;

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
        response.Content!.Message.Should().Be(nameof(Pong));
    }
}