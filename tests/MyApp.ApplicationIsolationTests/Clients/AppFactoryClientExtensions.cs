using MyApp.Server.Infrastructure.Auth;

namespace MyApp.ApplicationIsolationTests.Clients;

public static class AppFactoryClientExtensions
{
    public static IApplicationClient CreateClientWithCredentials(this AppFactory appFactory, int userId, string username, string email)
    {
        var jwtGenerator = appFactory.Services.GetRequiredService<IJwtGenerator>();
        var accessToken = jwtGenerator.CreateAccessToken(userId, username, email);
        return appFactory.CreateClientWithToken(accessToken);
    }

    public static IApplicationClient CreateClientWithToken(this AppFactory appFactory, string accessToken)
    {
        var httpClient = appFactory.CreateClient().SetAuthorizationHeader(accessToken);
        return RestService.For<IApplicationClient>(httpClient);
    }

    public static IApplicationGraphQLClient CreateGraphQLClientWithToken(this AppFactory appFactory, string accessToken)
        => appFactory.CreateGraphQLClient(c => c.SetAuthorizationHeader(accessToken));
}
