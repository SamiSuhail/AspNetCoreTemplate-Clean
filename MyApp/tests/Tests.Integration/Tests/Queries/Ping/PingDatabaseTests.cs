﻿using MyApp.Presentation.Interfaces.Http.Queries.Ping;
using MyApp.Domain.Access.Scope;

namespace MyApp.Tests.Integration.Tests.Queries.Ping;

public class PingDatabaseTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenUserIsUnauthorized_ReturnsError()
    {
        // Act
        var response = await UnauthorizedAppClient.PingDatabase();

        // Assert
        response.AssertUnauthorizedError();
    }

    [Fact]
    public async Task GivenTokenHasExpired_ReturnsError()
    {
        // Arrange
        string accessToken = ScopedServices.ArrangeExpiredAccessToken(User);
        var client = AppFactory.ArrangeClientWithToken(accessToken);

        // Act
        var response = await client.PingDatabase();

        // Assert
        response.AssertUnauthorizedError();
    }

    [Fact]
    public async Task GivenTokenIsInvalid_ReturnsError()
    {
        // Arrange
        var jwtGenerator = ScopedServices.ArrangeJwtGeneratorWithInvalidPrivateKey();
        var accessToken = jwtGenerator.CreateAccessToken(User.Entity.Id, User.Entity.Username, User.Entity.Email, ScopeCollection.Empty);
        var client = AppFactory.ArrangeClientWithToken(accessToken);

        // Act
        var response = await client.PingDatabase();

        // Assert
        response.AssertUnauthorizedError();
    }

    [Fact]
    public async Task GivenUserIsAuthorized_ReturnsResponse()
    {
        // Act
        var response = await AppClient.PingDatabase();

        // Assert
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        response.Content!.Message.Should().Be(Pong.DefaultMessage);
    }
}
