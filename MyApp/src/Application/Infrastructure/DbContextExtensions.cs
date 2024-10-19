using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.Infra.Instance;
using MyApp.Domain.Infra.Instance.Failures;

namespace MyApp.Application.Infrastructure;

public static class DbContextExtensions
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

    public static async Task<int> GetInstanceId(
        this IBaseDbContext dbContext,
        string? instanceName,
        CancellationToken cancellationToken)
    {
        var instanceNameToUse = instanceName ?? InstanceConstants.DefaultInstanceName;

        var instance = await dbContext.Set<InstanceEntity>()
            .Where(x => x.Name == instanceNameToUse)
            .Select(x => new { x.Id })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw InstanceNotFoundFailure.Exception();

        return instance.Id;
    }

    public static async Task<bool> NoneAsync<TEntity>(
        this IQueryable<TEntity> entities,
        CancellationToken cancellationToken = default)
        => !await entities.AnyAsync(cancellationToken);
    public static async Task<bool> NoneAsync<TEntity>(
        this IQueryable<TEntity> entities,
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => !await entities.AnyAsync(predicate, cancellationToken);
}
