namespace MyApp.Tests.System.Tests;

public class SampleTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Run()
    {
        var client = await CreateRandomUserClient();
    }
}
