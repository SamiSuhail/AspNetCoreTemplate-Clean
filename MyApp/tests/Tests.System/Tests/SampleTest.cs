using MyApp.Tests.System.Core;

namespace MyApp.Tests.System.Tests;

public class SampleTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Run()
    {
        await Task.CompletedTask;
        // To make system tests work we need to be able to create accounts on the fly for the same email
        // For that we need:
        // - multiple users with same email (organisation/tenant entities?)
        // - mailhog uses relay
        // - test email account

        // Other todos: reduce noise in errors
    }
}
