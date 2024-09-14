using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Utilities;

public static class ArrangeHelper
{
    public static async Task<UserEntity> ArrangeRandomUser(this IBaseDbContext dbContext)
    {
        var user = UserEntity.Create(RandomData.Username, RandomData.Password, RandomData.Email);
        dbContext.Add(user);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return user;
    }
}
