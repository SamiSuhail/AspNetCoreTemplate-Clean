using System.Linq.Expressions;

namespace MyApp.Application.Infrastructure;

public static class DbContextExtensions
{
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
