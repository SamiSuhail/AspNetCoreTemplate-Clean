using MyApp.Application.Infrastructure.Abstractions.Auth;

namespace MyApp.Tests.Integration.Utilities.Assert;

public static class AssertAccessTokenExtensions
{
    public static void AssertAccessTokenScopesEquivalentTo(this IServiceProvider sp, string tokenInput, string[] scopeNames)
    {
        var jwtReader = sp.GetRequiredService<IJwtReader>();
        var accessToken = jwtReader.ReadAccessToken(tokenInput);
        accessToken.Should().NotBeNull();
        accessToken!.Scopes.Should().BeEquivalentTo(scopeNames);
    }
}
