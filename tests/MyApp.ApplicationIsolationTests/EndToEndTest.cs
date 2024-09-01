using MyApp.ApplicationIsolationTests.Core;

namespace MyApp.ApplicationIsolationTests;

public class EndToEndTest : IClassFixture<AppFactory>
{
    private readonly AppFactory _appFactory;
    private readonly MockBag _mockBag;
    private readonly HttpClient _httpClient;

    public EndToEndTest(AppFactory appFactory)
    {
        _appFactory = appFactory;
        _mockBag = appFactory.MockBag;
        _httpClient = appFactory.HttpClient;
    }

    [Fact]
    public async Task HappyPathTest()
    {
    }
}
