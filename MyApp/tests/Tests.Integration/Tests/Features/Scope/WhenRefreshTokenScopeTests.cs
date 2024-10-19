using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Interfaces.Commands.Auth.RefreshToken;
using MyApp.Domain.Access.Scope;

namespace MyApp.Tests.Integration.Tests.Features.Scope;

public class WhenRefreshTokenScopeTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenTokenHasScopes_ReturnsScopes()
    {
        // Arrange
        var scopeName = await ArrangeDbContext.ArrangeRandomScope(User.Entity.Id);
        var accessToken = ArrangeTokenWithScopes([scopeName]);

        // Act
        var response = await AppClient.RefreshToken(new(accessToken, User.RefreshToken));

        // Assert
        AssertResponseScopes(response, [scopeName]);
    }

    [Fact]
    public async Task GivenTokenHasNoScopes_ReturnsNoScopes()
    {
        // Arrange
        var accessToken = ArrangeTokenWithScopes([]);

        // Act
        var response = await AppClient.RefreshToken(new(accessToken, User.RefreshToken));

        // Assert
        AssertResponseScopes(response, []);
    }

    [Fact]
    public async Task GivenTokenHasScopes_AndScopesDoNotExist_ReturnsOnlyValidScopes()
    {
        // Arrange
        var existingScope = await ArrangeDbContext.ArrangeRandomScope(User.Entity.Id);
        var nonExistingScope = RandomData.ScopeName;

        var accessToken = ArrangeTokenWithScopes([existingScope, nonExistingScope]);

        // Act
        var response = await AppClient.RefreshToken(new(accessToken, User.RefreshToken));

        // Assert
        AssertResponseScopes(response, [existingScope]);
    }

    private string ArrangeTokenWithScopes(string[] scopeNames)
    {
        return ScopedServices.GetRequiredService<IJwtGenerator>()
            .CreateAccessToken(User.Entity.Id, User.Entity.Username, User.Entity.Email, ScopeCollection.Create(scopeNames));
    }

    private void AssertResponseScopes(IApiResponse<RefreshTokenResponse> response, string[] scopeNames)
    {
        response.AssertSuccess();
        response.Content.Should().NotBeNull();

        ScopedServices.AssertAccessTokenScopesEquivalentTo(response.Content!.AccessToken, scopeNames);
    }
}
