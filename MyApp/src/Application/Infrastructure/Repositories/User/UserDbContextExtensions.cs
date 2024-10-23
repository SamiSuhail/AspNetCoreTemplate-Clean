using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.Auth.User;

namespace MyApp.Application.Infrastructure.Repositories.User;

public static class UserDbContextExtensions
{
    public static async Task<UserEntity?> FindUserOrDefault(
        this IQueryable<UserEntity> users,
        int userId,
        CancellationToken cancellationToken = default)
        => await users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public static async Task<UserEntity> FindUser(
        this IQueryable<UserEntity> users,
        int userId,
        CancellationToken cancellationToken = default)
        => await users.FindUserOrDefault(userId, cancellationToken)
                ?? throw UserNotFoundFailure.Exception();
}
