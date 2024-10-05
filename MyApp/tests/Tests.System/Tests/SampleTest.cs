using MyApp.Tests.System.Core;

namespace MyApp.Tests.System.Tests;

public class SampleTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Run()
    {
        await Task.CompletedTask;
    }
}
