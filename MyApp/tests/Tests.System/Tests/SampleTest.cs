namespace MyApp.Tests.System.Tests;

public class SampleTest(TestFixture fixture) : AuthenticatedBaseTest(fixture)
{
    [Fact]
    public async Task Run()
    {
        await Task.CompletedTask;
    }
}
