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

    public static IApplicationClient CreateClientWithToken(this AppFactory appFactory, string accessToken)
    {
        var httpClient = appFactory.CreateClient().SetAuthorizationHeader(accessToken);
        return RestService.For<IApplicationClient>(httpClient);
    }

    public static IApplicationGraphQLClient CreateGraphQLClientWithToken(this AppFactory appFactory, string accessToken)
        => appFactory.CreateGraphQLClient(c => c.SetAuthorizationHeader(accessToken));
}
