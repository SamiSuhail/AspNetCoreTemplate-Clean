using MyApp.ApplicationIsolationTests.Core.FakeExternalServices;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Infrastructure.Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Testcontainers.PostgreSql;
using DbDeployHelpers = MyApp.DbDeploy.Helpers;
using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.ApplicationIsolationTests.Core;

public class AppFactory : WebApplicationFactory<ProgramApi>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;


    private string _connectionString = string.Empty;
    // The fields are initialized in InitializeAsync method
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AppFactory()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base()
    {
        MockBag = new();
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithCleanUp(true)
            .WithDatabase($"test_run_{Guid.NewGuid()}")
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();
    }

    public MockBag MockBag { get; set; }
    public HttpClient HttpClient { get; private set; }
    public IApplicationClient AppClient { get; private set; }
    public IApplicationGraphQLClient GraphQLClient { get; private set; }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        _connectionString = _postgreSqlContainer.GetConnectionString();

        DbDeployHelpers.DeployDatabase(_connectionString);

        HttpClient = CreateClient();
        AppClient = RestService.For<IApplicationClient>(HttpClient);
        GraphQLClient = CreateGraphQLClient();
    }

    public IApplicationGraphQLClient CreateGraphQLClient(Action<HttpClient>? configureClient = null)
    {
        return new ServiceCollection()
            .AddApplicationGraphQLClient()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new($"{HttpClient.BaseAddress!.AbsoluteUri}graphql");
                configureClient?.Invoke(c);
            },
                cb => cb.ConfigurePrimaryHttpMessageHandler(() => Server.CreateHandler()))
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IApplicationGraphQLClient>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configure =>
        {
            builder.UseSetting($"{ConnectionStringsSettings.SectionName}:{nameof(ConnectionStringsSettings.Database)}", _connectionString);
        });
        builder.ConfigureServices(services =>
        {
            services.ReplaceService<IEmailSender>(new FakeEmailSender());
            services.ReplaceWithMock<IMessageProducer>(MockBag);
            services.ReplaceStandardMocks(MockBag);
            services.AddSingleton(new ConnectionStringsSettings() { Database = _connectionString });
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}
