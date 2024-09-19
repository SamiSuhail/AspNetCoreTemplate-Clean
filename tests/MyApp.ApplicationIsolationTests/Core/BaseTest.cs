using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Core;

public abstract class BaseTest(AppFactory appFactory) : IAsyncLifetime, IClassFixture<AppFactory>
{
    public virtual async Task InitializeAsync()
    {
        await appFactory.InitializeServices();
        AppFactory = appFactory;
        ScopedServices = appFactory.ScopedServices;
        MockBag = appFactory.MockBag;
        ArrangeDbContext = CreateDbContext();
        AssertDbContext = CreateDbContext();
        User = await TestUser.CreateTestUser(ScopedServices);
        UnauthorizedAppClient = RestService.For<IApplicationClient>(appFactory.CreateClient());
        AppClient = appFactory.CreateClientWithToken(User.AccessToken);
        UnauthorizedGraphQLClient = appFactory.CreateGraphQLClient();
        GraphQLClient = appFactory.CreateGraphQLClientWithToken(User.AccessToken);
    }

    public AppFactory AppFactory { get; private set; } = default!;
    public MockBag MockBag { get; private set; } = default!;
    public IApplicationClient UnauthorizedAppClient { get; private set; } = default!;
    public IApplicationClient AppClient { get; private set; } = default!;
    public IApplicationGraphQLClient UnauthorizedGraphQLClient { get; private set; } = default!;
    public IApplicationGraphQLClient GraphQLClient { get; private set; } = default!;

    public TestUser User { get; set; } = default!;

    public IServiceProvider ScopedServices { get; private set; } = default!;

    // Exposing the below two properties to avoid accidental sharing of DbContext between arrange and assert
    // This is done to avoid unexpected results due to entity tracking
    public ITransientDbContext ArrangeDbContext { get; private set; } = default!;
    public ITransientDbContext AssertDbContext { get; private set; } = default!;

    public ITransientDbContext CreateDbContext() => ScopedServices.GetRequiredService<ITransientDbContext>();

    public virtual Task DisposeAsync()
        => appFactory.DisposeServices();
}
