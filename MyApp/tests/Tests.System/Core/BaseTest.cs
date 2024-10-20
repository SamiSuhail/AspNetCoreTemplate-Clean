
using MyApp.Tests.Utilities.Clients;

namespace MyApp.Tests.System.Core;

public class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>, IAsyncLifetime
{
    public TestFixture Fixture { get; } = fixture;

    public async Task InitializeAsync()
    {
        await Fixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await Fixture.DisposeAsync();
    }
}
