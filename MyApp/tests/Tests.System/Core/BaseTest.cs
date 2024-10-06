
namespace MyApp.Tests.System.Core;

public class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>, IAsyncLifetime
{
    private readonly TestFixture _fixture = fixture;

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
