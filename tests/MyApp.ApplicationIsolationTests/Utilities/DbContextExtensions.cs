using MyApp.Server.Application.Utilities;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Utilities;

public static class DbContextExtensions
{
    public static async Task<UserEntity> GetUser(this IBaseDbContext dbContext, int userId)
    {
        var user = await dbContext.GetUserOrDefault(userId);
        user.Should().NotBeNull();
        return user!;
    }

    public static async Task<UserEntity?> GetUserOrDefault(this IBaseDbContext dbContext, int userId)
    {
        return await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Include(u => u.UserConfirmation)
            .Include(u => u.PasswordResetConfirmation)
            .Include(u => u.EmailChangeConfirmation)
            .FindUserOrDefault(userId, CancellationToken.None);
    }
}
