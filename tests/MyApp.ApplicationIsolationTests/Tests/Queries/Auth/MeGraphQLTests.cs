namespace MyApp.ApplicationIsolationTests.Tests.Queries.Auth;

public class MeGraphQLTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenUserIsUnauthorized_ThenReturnsError()
    {
        // Act
        var response = await UnauthorizedGraphQLClient.Me.ExecuteAsync();

        // Assert
        response.AssertUnauthorized();
    }

    [Fact]
    public async Task GivenUserIsAuthorized_ThenReturnsResponse()
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
}
