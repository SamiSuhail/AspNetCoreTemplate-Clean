
namespace MyApp.InfrastructureTests.Core;

public class TestFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await GlobalContext.InitializeAsync();
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
