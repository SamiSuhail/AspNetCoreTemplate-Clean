using Microsoft.Extensions.Configuration;

namespace MyApp.InfrastructureTests.Core;

public abstract class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>
{
    public TestFixture Fixture { get; } = fixture;
    public IConfiguration Configuration { get; } = GlobalContext.Configuration;
}
