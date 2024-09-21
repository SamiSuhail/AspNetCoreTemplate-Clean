using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MyApp.Server;
using MyApp.Server.Infrastructure.Abstractions;
using MyApp.Server.Infrastructure.BackgroundJobs;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Core;

public class AppFactory : WebApplicationFactory<ProgramApi>
{
    private IServiceScope _scope = default!;
    public IServiceProvider ScopedServices { get; private set; } = default!;
    public MockBag MockBag { get; } = new();

    public async Task InitializeServices()
    {
        await GlobalContext.InitializeAsync();
        _scope = Services.CreateScope();
        ScopedServices = _scope.ServiceProvider;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting($"{ConnectionStringsSettings.SectionName}:{nameof(ConnectionStringsSettings.Database)}", GlobalContext.ConnectionString);
        builder.UseSetting($"{BackgroundJobsSettings.SectionName}:{nameof(BackgroundJobsSettings.Enabled)}", "false");
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