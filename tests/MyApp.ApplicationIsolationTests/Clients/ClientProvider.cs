using MyApp.Server.Infrastructure.Auth;

namespace MyApp.ApplicationIsolationTests.Clients;

public static class ClientProvider
{
    public static void Initialize(AppFactory appFactory)
    {
        UnauthorizedAppClient = RestService.For<IApplicationClient>(appFactory.CreateClient());
        UnauthorizedGraphQLClient = appFactory.CreateGraphQLClient();
        AppClient = appFactory.CreateClientWithToken(TestUser.AccessToken);
        GraphQLClient = appFactory.CreateGraphQLClientWithToken(TestUser.AccessToken);
    }

    public static IApplicationClient UnauthorizedAppClient { get; private set; } = default!;
    public static IApplicationClient AppClient { get; private set; } = default!;
    public static IApplicationGraphQLClient UnauthorizedGraphQLClient { get; private set; } = default!;
    public static IApplicationGraphQLClient GraphQLClient { get; private set; } = default!;

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
