namespace MyApp.Application.Infrastructure.Abstractions.Database;

public interface IBaseDbContext : IDisposable, IAsyncDisposable
{
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void AddRange<TEntity>(params TEntity[] entities) where TEntity : class;
    void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    void Remove<TEntity>(TEntity entity) where TEntity : class;
    void RemoveRange<TEntity>(params TEntity[] entities) where TEntity : class;
    void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    IQueryable<TEntity> Set<TEntity>() where TEntity : class;
    ValueTask<TEntity?> FindAsync<TEntity>(int id, CancellationToken cancellationToken = default) where TEntity : class;
    ValueTask<TEntity?> FindAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : class;
    ValueTask<TEntity?> FindAsync<TEntity>(object?[]? keyValues, CancellationToken cancellationToken = default) where TEntity : class;

    Task WrapInTransaction(Func<Task> action, CancellationToken cancellationToken = default);
}
