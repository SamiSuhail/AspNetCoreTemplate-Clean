using MyApp.Domain.Access.Scope;

namespace MyApp.Tests.Integration.Tests;

public class AppAuthTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenAuthorized_ReturnsAnonymousOnlyError()
    {
        // Act
        var response = await AppClient.Login(RandomRequests.Login);

        // Assert
        response.AssertAnonymousOnlyError();
    }


    [Fact]
    public async Task GivenUnauthorized_ReturnsUnauthorizedError()
    {
        // Act
        var response = await UnauthorizedAppClient.ChangeEmail(RandomRequests.ChangeEmail);

        // Assert
        response.AssertUnauthorizedError();
    }

    [Fact]
    public async Task GivenUserIsUnauthorized_ReturnsUnauthorizedError()
    {
        // Act
        var response = await UnauthorizedGraphQLClient.PingDatabase.ExecuteAsync();

        // Assert
        response.AssertUnauthorizedError();
    }

    [Fact]
    public async Task GivenTokenHasExpired_ReturnsUnauthorizedError()
    {
        // Arrange
        var accessToken = ScopedServices.ArrangeExpiredAccessToken(User);
        var graphQlClient = AppFactory.ArrangeGraphQLClientWithToken(accessToken);

        // Act
        var response = await graphQlClient.PingDatabase.ExecuteAsync();

        // Assert
        response.AssertUnauthorizedError();
    }

    [Fact]
    public async Task GivenTokenIsInvalid_ReturnsUnauthorizedError()
    {
        // Arrange
        var jwtGenerator = ScopedServices.ArrangeJwtGeneratorWithInvalidPrivateKey();
        var accessToken = jwtGenerator.CreateAccessToken(User.Entity.Id, User.Entity.Username, User.Entity.Email, ScopeCollection.Empty);
        var graphQlClient = AppFactory.ArrangeGraphQLClientWithToken(accessToken);

        // Act
        var response = await graphQlClient.PingDatabase.ExecuteAsync();

        // Assert
        response.AssertUnauthorizedError();
    }

    [Fact]
    public async Task GivenRequiredScopeMissing_ReturnsForbiddenError()
    {
        // Arrange
        var user = await ArrangeDbContext.ArrangeRandomConfirmedUser();
        var client = AppFactory.ArrangeClientWithCredentials(user.Id, user.Username, user.Email);

        // Act
        var response = await client.CreateInstance(RandomRequests.CreateInstance);

        // Assert
        response.AssertForbiddenError();
    }
}
