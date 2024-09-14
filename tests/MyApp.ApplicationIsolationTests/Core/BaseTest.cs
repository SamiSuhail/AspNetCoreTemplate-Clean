using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Core;

[Collection(nameof(AppFactory))]
public abstract class BaseTest(AppFactory appFactory) : IAsyncLifetime
{
    private readonly IServiceScope _scope = appFactory.Services.CreateScope();

    public virtual async Task InitializeAsync()
    {
        ScopedServices = _scope.ServiceProvider;
        ArrangeDbContext = CreateDbContext();
        AssertDbContext = CreateDbContext();
        await Task.CompletedTask;
    }
    public IServiceProvider ScopedServices { get; set; } = default!;

    // Exposing the below two properties to avoid accidental sharing of DbContext between arrange and assert
    // This is done to avoid unexpected results due to entity tracking
    public ITransientDbContext ArrangeDbContext { get; set; } = default!;
    public ITransientDbContext AssertDbContext { get; set; } = default!;
    public ITransientDbContext CreateDbContext() => ScopedServices.GetRequiredService<ITransientDbContext>();

    public async Task DisposeAsync()
    {
        _scope.Dispose();
        await Task.CompletedTask;
    }
}
