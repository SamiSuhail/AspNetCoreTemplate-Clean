using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Domain.Access.Scope;
using MyApp.Domain.Auth.User;

namespace MyApp.Tests.Integration.Utilities;

public class TestUser
{
    private TestUser() { }

    public string AccessToken { get; private set; } = default!;
    public string RefreshToken { get; private set; } = default!;
    public string Password { get; private set; } = default!;
    public UserEntity Entity { get; private set; } = default!;

    public static async Task<TestUser> CreateTestUser(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITransientDbContext>();
        var (user, password) = await dbContext.ArrangeRandomConfirmedUserWithPassword();

        await dbContext.ArrangeScopesForUser(CustomScopes.All, user.Id);
        var jwtGenerator = sp.GetRequiredService<IJwtGenerator>();
        var accessToken = jwtGenerator.CreateAccessToken(user.Id, user.Username, user.Email, ScopeCollection.Create(CustomScopes.All));
        var refreshToken = jwtGenerator.CreateRefreshToken(user.Id, user.RefreshTokenVersion);
        return new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Password = password,
            Entity = user,
        };
    }
}
