namespace MyApp.Tests.System.Utilities.Arrange;

public static class HttpClientFactoryExtensions
{
    public static async Task<HttpClient> CreateClientForUser(this IHttpClientFactory clientFactory, UserCredentials credentials, string instanceName)
    {
        var client = clientFactory.CreateClient(nameof(BaseTest.UnauthorizedAppClient));
        var (username, password, _) = credentials;

        var response = await client.ToApplicationClient()
            .Login(new(username, password, Scopes: []), instanceName);
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        var accessToken = response.Content!.AccessToken;

        client.SetAuthorizationHeader(accessToken);
        return client;
    }
}
