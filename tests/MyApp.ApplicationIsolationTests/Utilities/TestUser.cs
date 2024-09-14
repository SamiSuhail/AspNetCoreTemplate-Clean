using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Utilities;

public static class TestUser
{
    public static string AccessToken = null!;
    public static int Id = default!;
    public const string Username = "Username";
    public const string Password = "Password1!";
    public const string Email = "user@email.com";
    public static DateTime CreatedAt = default!;

    public static async Task InitializeTestUser(this IServiceProvider sp)
    {
        if (Id > 0)
            return;
        using var scope = sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITransientDbContext>();
        var user = await dbContext.CreateConfirmedTestUser(Username, Password, Email);
        Id = user.Id;
        CreatedAt = user.CreatedAt;

        var jwtGenerator = sp.GetRequiredService<IJwtGenerator>();
        AccessToken = jwtGenerator.Create(Id, Username, Email);
    }

    public static async Task<UserEntity> CreateConfirmedTestUser(this IBaseDbContext dbContext, string username, string password, string email)
    {
        var user = UserEntity.Create(username, password, email);
        user.ConfirmEmail();
        dbContext.Add(user);
        dbContext.Remove(user.EmailConfirmation!);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return user;
    }
}
