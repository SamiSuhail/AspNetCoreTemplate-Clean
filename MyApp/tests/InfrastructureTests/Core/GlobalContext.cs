using Microsoft.Extensions.Configuration;
using MyApp.Infrastructure.Database;
using Testcontainers.PostgreSql;
using DbDeployHelpers = MyApp.DbDeploy.Helpers;

namespace MyApp.InfrastructureTests.Core;

public static class GlobalContext
{
    private static readonly PostgreSqlContainer _postgreSqlContainer;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static bool _isInitialized = false;

    static GlobalContext()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithCleanUp(true)
            .WithDatabase($"test_run_{Guid.NewGuid()}")
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();
    }

    public static IConfigurationRoot Configuration { get; private set; } = default!;

    public static string ConnectionString { get; private set; } = default!;

    public static async Task InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                _semaphore.Release();
                return;
            }

            await _postgreSqlContainer.StartAsync();
            ConnectionString = _postgreSqlContainer.GetConnectionString();
            DbDeployHelpers.DeployDatabase(ConnectionString);

            Configuration = new ConfigurationBuilder()
                .AddInfrastructureAppsettings()
                .AddInMemoryCollection([
                    new($"{BaseSettings<ConnectionStringsSettings>.SectionName}:{nameof(ConnectionStringsSettings.Database)}", ConnectionString),
                    ])
                .Build();

            _isInitialized = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
