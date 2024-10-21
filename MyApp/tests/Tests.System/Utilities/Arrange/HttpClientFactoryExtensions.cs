namespace MyApp.Tests.System.Utilities.Arrange;

public static class HttpClientFactoryExtensions
{
    public static async Task<HttpClient> CreateClientForUser(this IHttpClientFactory clientFactory, UserCredentials credentials)
    {
        var client = clientFactory.CreateClient(nameof(BaseTest.UnauthorizedAppClient));
        var (username, password, instanceName) = credentials;

        var response = await client.ToApplicationClient()
            .Login(new(username, password, Scopes: []), instanceName);
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        var accessToken = response.Content!.AccessToken;

        client.SetAuthorizationHeader(accessToken);
        return client;
    }
}
