namespace MyApp.Tests.Integration.Clients;

public static class ApplicationGraphQLClient
{
    public static IApplicationGraphQLClient CreateGraphQLClient(this AppFactory appFactory, Action<HttpClient>? configureClient = null)
    {
        return new ServiceCollection()
            .AddApplicationGraphQLClient()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new($"{appFactory.CreateClient().BaseAddress!.AbsoluteUri}graphql");
                configureClient?.Invoke(c);
            },
            cb => cb.ConfigurePrimaryHttpMessageHandler(() => appFactory.Server.CreateHandler()))
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IApplicationGraphQLClient>();
    }
}
