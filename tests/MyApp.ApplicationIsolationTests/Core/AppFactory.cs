using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Infrastructure.Messaging;
using Testcontainers.PostgreSql;
using DbDeployHelpers = MyApp.DbDeploy.Helpers;

namespace MyApp.ApplicationIsolationTests.Core;

public class AppFactory : WebApplicationFactory<ProgramApi>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private string _connectionString = default!;
    public AppFactory()
        : base()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithCleanUp(true)
            .WithDatabase($"test_run_{Guid.NewGuid()}")
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        _connectionString = _postgreSqlContainer.GetConnectionString();
        DbDeployHelpers.DeployDatabase(_connectionString);
        await Initialize(this);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting($"{ConnectionStringsSettings.SectionName}:{nameof(ConnectionStringsSettings.Database)}", _connectionString);
        builder.ConfigureServices(services =>
        {
            services.AddMassTransitTestHarness();
            services.ReplaceWithMock<IMessageProducer>(MockBehavior.Loose);
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}

[CollectionDefinition(nameof(AppFactory), DisableParallelization = false)]
public class DatabaseCollection : ICollectionFixture<AppFactory>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}