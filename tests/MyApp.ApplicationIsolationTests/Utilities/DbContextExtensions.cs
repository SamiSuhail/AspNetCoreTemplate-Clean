using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.ApplicationIsolationTests.Utilities;

public static class DbContextExtensions
{
    public static async Task<UserEntity?> GetUser(this IBaseDbContext dbContext, int userId)
    {
        return await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Include(u => u.EmailConfirmation)
            .Include(u => u.PasswordResetConfirmation)
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
    }
}
