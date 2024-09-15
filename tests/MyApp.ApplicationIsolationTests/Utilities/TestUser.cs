using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Utilities;

public static class TestUser
{
    public static string AccessToken = default!;
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
        var user = await dbContext.ArrangeConfirmedUser(Username, Password, Email);
        Id = user.Id;
        CreatedAt = user.CreatedAt;

        var jwtGenerator = sp.GetRequiredService<IJwtGenerator>();
        AccessToken = jwtGenerator.Create(Id, Username, Email);
    }
}
