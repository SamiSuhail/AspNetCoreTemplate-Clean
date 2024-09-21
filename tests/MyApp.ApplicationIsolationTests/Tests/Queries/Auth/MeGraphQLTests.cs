using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Abstractions.Auth;

namespace MyApp.ApplicationIsolationTests.Tests.Queries.Auth;

public class MeGraphQLTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenUserIsAuthorized_ReturnsResponse()
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
    public async Task GivenUserIsUnauthorized_ReturnsError()
    {
        // Act
        var response = await UnauthorizedGraphQLClient.Me.ExecuteAsync();

        // Assert
        response.AssertUnauthorized();
    }

    [Fact]
    public async Task GivenUserNotFound_ReturnsFailure()
    {
        // Arrange
        var jwtGenerator = ScopedServices.GetRequiredService<IJwtGenerator>();
        var accessToken = jwtGenerator.CreateAccessToken(userId: int.MaxValue, User.Entity.Username, User.Entity.Email);
        var client = AppFactory.ArrangeGraphQLClientWithToken(accessToken);

        // Act
        var response = await client.Me.ExecuteAsync();

        // Assert
        response.AssertSingleError(UserIdNotFoundFailure.Key, UserIdNotFoundFailure.Message);
    }
}
