using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Domain.Access.Scope;

namespace MyApp.Tests.Integration.Tests.Queries.Auth;

public class MeGraphQLTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ReturnsResponse()
    {
        // Act
        var response = await GraphQLClient.Me.ExecuteAsync();

        // Assert
        response.AssertSuccess();
        response.Data!.Me.Should().NotBeNull();
        var user = response.Data.Me.User;
        user.Should().NotBeNull();
        user.Username.Should().Be(User.Entity.Username);
        user.Email.Should().Be(User.Entity.Email);
        user.Id.Should().Be(User.Entity.Id);
        user.CreatedAt.Should().BeCloseTo(User.Entity.CreatedAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task GivenUserNotFound_ReturnsFailure()
    {
        // Arrange
        var jwtGenerator = ScopedServices.GetRequiredService<IJwtGenerator>();
        var accessToken = jwtGenerator.CreateAccessToken(userId: int.MaxValue, User.Entity.Username, User.Entity.Email, ScopeCollection.Empty);
        var client = AppFactory.ArrangeGraphQLClientWithToken(accessToken);

        // Act
        var response = await client.Me.ExecuteAsync();

        // Assert
        response.AssertUserNotFoundFailure();
    }
}
