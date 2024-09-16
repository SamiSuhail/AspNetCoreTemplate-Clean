using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Utilities.Arrange;

public static class UserDbContextExtensions
{
    public static async Task<UserEntity> ArrangeRandomConfirmedUser(this IBaseDbContext dbContext)
    {
        var user = await dbContext.ArrangeRandomUnconfirmedUser();
        await dbContext.ConfirmUser(user);
        return user;
    }

    public static async Task<UserEntity> ArrangeRandomUnconfirmedUser(this IBaseDbContext dbContext)
        => await dbContext.ArrangeUnconfirmedUser(RandomData.Username, RandomData.Password, RandomData.Email);

    public static async Task<UserEntity> ArrangeConfirmedUser(this IBaseDbContext dbContext, string username, string password, string email)
    {
        var user = await dbContext.ArrangeUnconfirmedUser(username, password, email);
        await dbContext.ConfirmUser(user);
        return user;
    }

    public static async Task<UserEntity> ArrangeUnconfirmedUser(this IBaseDbContext dbContext, string username, string password, string email)
    {
        var user = UserEntity.Create(username, password, email);
        dbContext.Add(user);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return user;
    }

    public static async Task ConfirmUser(this IBaseDbContext dbContext, UserEntity user)
    {
        user.ConfirmUserRegistration();
        dbContext.Remove(user.UserConfirmation!);
        await dbContext.SaveChangesAsync(CancellationToken.None);
    }
}
