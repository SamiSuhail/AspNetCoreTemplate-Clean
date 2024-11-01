namespace MyApp.Tests.System.Core;

public class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>, IAsyncLifetime
{
    protected IHttpClientFactory ClientFactory { get; private set; } = default!;
    public TestFixture Fixture { get; } = fixture;
    public IApplicationAdminClient AdminAppClient { get; private set; } = default!;
    public IApplicationClient UnauthorizedAppClient { get; private set; } = default!;

    public virtual async Task InitializeAsync()
    {
        await Fixture.InitializeAsync();
        ClientFactory = Fixture.Services.GetRequiredService<IHttpClientFactory>();

        AdminAppClient = CreateAdminClient()
            .ToApplicationAdminClient();
        UnauthorizedAppClient = CreateUnauthorizedClient()
            .ToApplicationClient();
    }

    public HttpClient CreateAdminClient()
        => ClientFactory.CreateClient(nameof(AdminAppClient));
    public HttpClient CreateUnauthorizedClient()
        => ClientFactory.CreateClient(nameof(UnauthorizedAppClient));

    public async Task DisposeAsync()
    {
        if (Fixture != null)
            await Fixture.DisposeAsync();
    }
}
