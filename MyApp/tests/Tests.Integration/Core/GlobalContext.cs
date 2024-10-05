using Testcontainers.PostgreSql;
using Tests.Utilities;
using DbDeployHelpers = MyApp.DbDeploy.Helpers;

namespace MyApp.Tests.Integration.Core;

public static class GlobalContext
{
    private static readonly PostgreSqlContainer _postgreSqlContainer;

    static GlobalContext()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithCleanUp(true)
            .WithDatabase($"test_run_{Guid.NewGuid()}")
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();
    }

    public static string ConnectionString { get; private set; } = default!;

    public static async Task InitializeAsync()
        => await GlobalInitializer.InitializeAsync(InitializeAsyncInternal);

    private static async Task InitializeAsyncInternal()
    {
        await _postgreSqlContainer.StartAsync();
        ConnectionString = _postgreSqlContainer.GetConnectionString();
        DbDeployHelpers.DeployDatabase(ConnectionString);
    }
}
