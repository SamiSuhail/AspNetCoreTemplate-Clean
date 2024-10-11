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
        // - multiple users with same email
        //  -> instance entity
        //      -> needs management so system tests can create instances on the fly
        //          -> needs instance:create scope

        // Tasks in order:
        //  1) create instances entity in DB
        //      a) with default seeded "system" instance
        //      b) cleanup flag on create (filtered? index)

        //  2) create scopes entity in DB
        //      a) with default "instance:create" scope seeded

        // - mailhog uses relay
        // - test email account

        // Other todos: reduce noise in errors
    }
}
