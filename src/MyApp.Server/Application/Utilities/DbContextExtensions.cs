using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Application.Utilities;

public static class DbContextExtensions
{
    public static async Task<UserEntity?> FindUserOrDefault(this IBaseDbContext dbContext, int userId, CancellationToken cancellationToken = default)
        => await dbContext.FindAsync<UserEntity>(userId, cancellationToken);
    public static async Task<UserEntity> FindUser(this IBaseDbContext dbContext, int userId, CancellationToken cancellationToken = default)
        => await dbContext.FindUserOrDefault(userId, cancellationToken)
                ?? throw UserIdNotFoundFailure.Exception();
}
