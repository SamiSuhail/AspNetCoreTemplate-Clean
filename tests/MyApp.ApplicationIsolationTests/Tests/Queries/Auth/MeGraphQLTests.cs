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
        user.Username.Should().Be(TestUser.Username);
        user.Email.Should().Be(TestUser.Email);
        user.Id.Should().Be(TestUser.Id);
        user.CreatedAt.Should().BeCloseTo(TestUser.CreatedAt, TimeSpan.FromMilliseconds(1));
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
        var accessToken = jwtGenerator.CreateAccessToken(userId: int.MaxValue, User.Username, User.Email);
        var client = AppFactory.CreateGraphQLClientWithToken(accessToken);

        // Act
        var response = await client.Me.ExecuteAsync();

        // Assert
        response.AssertSingleError(UserIdNotFoundFailure.Key, UserIdNotFoundFailure.Message);
    }
}
