using MyApp.Server.Domain.Auth.User;
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
        User = (await CreateDbContext().GetUser(TestUser.Id))!;
        await Task.CompletedTask;
    }
    public IServiceProvider ScopedServices { get; private set; } = default!;

    // Exposing the below two properties to avoid accidental sharing of DbContext between arrange and assert
    // This is done to avoid unexpected results due to entity tracking
    public ITransientDbContext ArrangeDbContext { get; private set; } = default!;
    public ITransientDbContext AssertDbContext { get; private set; } = default!;
    public UserEntity User { get; private set; } = default!;

    public ITransientDbContext CreateDbContext() => ScopedServices.GetRequiredService<ITransientDbContext>();

    public virtual async Task DisposeAsync()
    {
        MockBag.Reset();
        _scope.Dispose();
        await Task.CompletedTask;
    }
}
