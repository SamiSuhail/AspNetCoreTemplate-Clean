using MyApp.Infrastructure.Database;
using MyApp.Tests.Utilities.Core;
using Testcontainers.PostgreSql;
using DbDeployHelpers = MyApp.DbDeploy.Helpers;

namespace MyApp.Tests.Integration.Core;

public static class GlobalContext
{
    public static Task Initialize { get; } = new AsyncLazy(InitializeAsyncInternal).Value;

    private static async Task InitializeAsyncInternal()
    {
        var postgreSqlContainer = new PostgreSqlBuilder()
            .WithCleanUp(true)
            .WithDatabase($"test_run_{Guid.NewGuid()}")
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();
        await postgreSqlContainer.StartAsync();
        var connectionString = postgreSqlContainer.GetConnectionString();
        DbDeployHelpers.DeployDatabase(connectionString);
        Environment.SetEnvironmentVariable($"{ConnectionStringsSettings.SectionName}:{nameof(ConnectionStringsSettings.Database)}", connectionString);
    }
}
