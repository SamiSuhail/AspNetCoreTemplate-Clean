namespace MyApp.Tests.System.Core;

public class TestFixture : IAsyncLifetime
{
    private AsyncServiceScope _serviceScope = default!;

    public IServiceProvider Services { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await GlobalContext.Initialize;
        _serviceScope = GlobalContext.Services.CreateAsyncScope();
        Services = _serviceScope.ServiceProvider;
    }

    public async Task DisposeAsync()
    {
        await _serviceScope.DisposeAsync();
    }
}
