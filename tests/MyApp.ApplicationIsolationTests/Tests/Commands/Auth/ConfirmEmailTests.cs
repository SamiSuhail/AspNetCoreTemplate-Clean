using MyApp.Server.Domain.Auth.User;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class ConfirmEmailTests(AppFactory appFactory) : BaseTest(appFactory), IAsyncLifetime
{
    private UserEntity _user = null!;
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var user = UserEntity.Create(RandomData.Username, RandomData.Password, RandomData.Email);
        ArrangeDbContext.Add(user);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);
    }
}
