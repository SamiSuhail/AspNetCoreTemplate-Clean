using Microsoft.Extensions.DependencyInjection;
using MyApp.Tests.Utilities.Clients;
using MyApp.Tests.Utilities.Clients.Extensions;
using Refit;

namespace MyApp.Tests.System.Core;

public class TestFixture : IAsyncLifetime
{
    private IServiceScope _serviceScope = default!;

    public IServiceProvider Services { get; private set; } = default!;
    public IApplicationClient AdminAppClient { get; private set; } = default!;
    public IApplicationClient UnauthorizedAppClient { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await GlobalContext.Initialize;
        _serviceScope = GlobalContext.Services.CreateAsyncScope();
        Services = _serviceScope.ServiceProvider;
        var clientFactory = Services.GetRequiredService<IHttpClientFactory>();
        AdminAppClient = clientFactory.CreateClient(nameof(AdminAppClient))
            .ToApplicationClient();
        UnauthorizedAppClient = clientFactory.CreateClient(nameof(UnauthorizedAppClient))
            .ToApplicationClient();
    }

    public async Task DisposeAsync()
    {
        _serviceScope.Dispose();
        await Task.CompletedTask;
    }
}
