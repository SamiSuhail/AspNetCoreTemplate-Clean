
namespace MyApp.Tests.System.Core;

public class TestFixture : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await GlobalContext.Initialize;
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
