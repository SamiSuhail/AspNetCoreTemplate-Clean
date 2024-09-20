using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;

namespace MyApp.Server.Application.Utilities;

public static class DbContextExtensions
{
    public static async Task<UserEntity?> FindUserOrDefault(this IQueryable<UserEntity> users, int userId, CancellationToken cancellationToken)
        => await users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    public static async Task<UserEntity> FindUser(this IQueryable<UserEntity> users, int userId, CancellationToken cancellationToken)
        => await users.FindUserOrDefault(userId, cancellationToken)
                ?? throw UserIdNotFoundFailure.Exception();
}
