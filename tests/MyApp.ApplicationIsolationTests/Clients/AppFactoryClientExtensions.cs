
using MyApp.Server.Infrastructure.Abstractions.Auth;

namespace MyApp.ApplicationIsolationTests.Clients;

public static class AppFactoryClientExtensions
{
    public static IApplicationClient ArrangeClientWithCredentials(this AppFactory appFactory, int userId, string username, string email)
    {
        var jwtGenerator = appFactory.Services.GetRequiredService<IJwtGenerator>();
        var accessToken = jwtGenerator.CreateAccessToken(userId, username, email);
        return appFactory.ArrangeClientWithToken(accessToken);
    }

    public static IApplicationClient ArrangeClientWithToken(this AppFactory appFactory, string accessToken)
    {
        var httpClient = appFactory.CreateClient().SetAuthorizationHeader(accessToken);
        return RestService.For<IApplicationClient>(httpClient);
    }

    public static IApplicationGraphQLClient ArrangeGraphQLClientWithToken(this AppFactory appFactory, string accessToken)
        => appFactory.CreateGraphQLClient(c => c.SetAuthorizationHeader(accessToken));
}
