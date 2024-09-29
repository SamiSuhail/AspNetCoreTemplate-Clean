using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MyApp.Server;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Infrastructure.BackgroundJobs;
using MyApp.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Core;

public class AppFactory : WebApplicationFactory<ProgramApi>
{
    private IServiceScope _scope = default!;
    public IServiceProvider ScopedServices { get; private set; } = default!;
    public MockBag MockBag { get; } = new();

    public async Task InitializeServices()
    {
        await GlobalContext.InitializeAsync();
        ProgramApi.ConfigurationOverrides ??= [
            new($"{ConnectionStringsSettings.SectionName}:{nameof(ConnectionStringsSettings.Database)}", GlobalContext.ConnectionString),
            new($"{BackgroundJobsSettings.SectionName}:{nameof(BackgroundJobsSettings.Enabled)}", "false"),
        ];
        _scope = Services.CreateScope();
        ScopedServices = _scope.ServiceProvider;
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddMassTransitTestHarness();
            services.ReplaceWithMock<IMessageProducer>(MockBag, MockBehavior.Loose);
        });
    }

    public Task DisposeServices()
    {
        _scope?.Dispose();
        MockBag.VerifyAll();
        MockBag.Reset();
        return Task.CompletedTask;
    }
}