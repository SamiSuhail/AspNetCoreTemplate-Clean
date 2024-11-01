using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Domain.Access.Scope;

namespace MyApp.Tests.Integration.Tests.Features.Scope;

public class WhenLoginScopeTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ReturnsTokenWithScopes()
    {
        // Arrange
        string scopeName = await ArrangeRandomScope();

        // Act
        var response = await UnauthorizedAppClient.Login(new(User.Entity.Username, User.Password, [scopeName]));

        // Assert
        AssertResponseScopes(response, [scopeName]);
    }

    [Fact]
    public async Task GivenScopesRequested_ReturnsOnlyRequestedScopes()
    {
        // Arrange
        string requestedScopeName = await ArrangeRandomScope();
        string otherScopeName = await ArrangeRandomScope();

        // Act
        var response = await UnauthorizedAppClient.Login(new(User.Entity.Username, User.Password, [requestedScopeName]));

        // Assert
        AssertResponseScopes(response, [requestedScopeName]);
    }

    [Fact]
    public async Task GivenNoScopesRequested_ReturnsTokenWithAllScopes()
    {
        // Act
        var response = await UnauthorizedAppClient.Login(new(User.Entity.Username, User.Password, []));

        // Assert
        AssertResponseScopes(response, CustomScopes.All);
    }

    private async Task<string> ArrangeRandomScope()
        => await ArrangeDbContext.ArrangeRandomScope(User.Entity.Id);

    private void AssertResponseScopes(IApiResponse<LoginResponse> response, string[] scopeNames)
    {
        response.AssertSuccess();
        response.Content.Should().NotBeNull();

        ScopedServices.AssertAccessTokenScopesEquivalentTo(response.Content!.AccessToken, scopeNames);
    }
}
