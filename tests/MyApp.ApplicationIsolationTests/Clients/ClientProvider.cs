namespace MyApp.ApplicationIsolationTests.Clients;

public static class ClientProvider
{
    public static async Task Initialize(AppFactory appFactory)
    {
        await appFactory.Services.InitializeTestUser();
        UnauthorizedAppClient = RestService.For<IApplicationClient>(appFactory.CreateClient());
        UnauthorizedGraphQLClient = appFactory.CreateGraphQLClient();
        var authorizedHttpClient = appFactory.CreateClient().SetAuthorizationHeader(TestUser.AccessToken);
        AppClient = RestService.For<IApplicationClient>(authorizedHttpClient);
        GraphQLClient = appFactory.CreateGraphQLClient(c => c.SetAuthorizationHeader(TestUser.AccessToken));
    }

    public static IApplicationClient UnauthorizedAppClient = default!;
    public static IApplicationClient AppClient = default!;
    public static IApplicationGraphQLClient UnauthorizedGraphQLClient = default!;
    public static IApplicationGraphQLClient GraphQLClient = default!;
}
