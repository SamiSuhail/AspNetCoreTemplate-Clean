namespace MyApp.Tests.System.Core;

public class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>, IAsyncLifetime
{
    private IHttpClientFactory _clientFactory = default!;

    public TestFixture Fixture { get; } = fixture;
    public IApplicationClient AdminAppClient { get; private set; } = default!;
    public IApplicationClient UnauthorizedAppClient { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await Fixture.InitializeAsync();
        _clientFactory = Fixture.Services.GetRequiredService<IHttpClientFactory>();

        AdminAppClient = CreateAdminClient()
            .ToApplicationClient();
        UnauthorizedAppClient = CreateUnauthorizedClient()
            .ToApplicationClient();
    }

    public HttpClient CreateAdminClient()
        => _clientFactory.CreateClient(nameof(AdminAppClient));
    public HttpClient CreateUnauthorizedClient()
        => _clientFactory.CreateClient(nameof(UnauthorizedAppClient));

    public async Task<IApplicationClient> CreateRandomUserClient()
    {
        var instanceName = await AdminAppClient.ArrangeRandomInstance();
        var userCredentials = await UnauthorizedAppClient.ArrangeRandomConfirmedUser(instanceName);
        var client = await _clientFactory.CreateClientForUser(userCredentials);
        return client.ToApplicationClient();
    }

    public async Task DisposeAsync()
    {
        if (Fixture != null)
            await Fixture.DisposeAsync();
    }
}
