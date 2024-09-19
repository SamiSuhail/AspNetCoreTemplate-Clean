using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Utilities;

public class TestUser
{
    private TestUser() { }

    public string AccessToken { get; private set; } = default!;
    public string Password { get; private set; } = default!;
    public UserEntity Entity { get; private set; } = default!;

    public static async Task<TestUser> CreateTestUser(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITransientDbContext>();
        var (user, password) = await dbContext.ArrangeRandomConfirmedUserWithPassword();

        var accessToken = sp.GetRequiredService<IJwtGenerator>()
            .CreateAccessToken(user.Id, user.Username, user.Email);
        return new()
        {
            AccessToken = accessToken,
            Password = password,
            Entity = user,
        };
    }
}
