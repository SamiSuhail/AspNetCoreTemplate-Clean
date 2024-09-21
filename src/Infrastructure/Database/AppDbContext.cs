using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Database;

namespace MyApp.Infrastructure.Database;

public class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options), IBaseDbContext, ITransientDbContext, IScopedDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyCustomNamingConvention();
        builder.ApplyCustomConfigurations();

        base.OnModelCreating(builder);
    }

    void IBaseDbContext.Add<TEntity>(TEntity entity) where TEntity : class
        => base.Add(entity);
    public void AddRange<TEntity>(params TEntity[] entities) where TEntity : class
        => base.AddRange(entities);
    public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        => base.AddRange(entities);

    void IBaseDbContext.Remove<TEntity>(TEntity entity) where TEntity : class
        => base.Remove(entity);
    public void RemoveRange<TEntity>(params TEntity[] entities) where TEntity : class
        => base.RemoveRange(entities);
    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        => base.RemoveRange(entities);

    IQueryable<TEntity> IBaseDbContext.Set<TEntity>() where TEntity : class
        => base.Set<TEntity>();
    ValueTask<TEntity?> IBaseDbContext.FindAsync<TEntity>(int id, CancellationToken cancellationToken) where TEntity : class
        => base.FindAsync<TEntity>([id], cancellationToken);
    ValueTask<TEntity?> IBaseDbContext.FindAsync<TEntity>(Guid id, CancellationToken cancellationToken) where TEntity : class
        => base.FindAsync<TEntity>([id], cancellationToken);
    ValueTask<TEntity?> IBaseDbContext.FindAsync<TEntity>(object?[]? keyValues, CancellationToken cancellationToken) where TEntity : class
        => base.FindAsync<TEntity>(keyValues, cancellationToken);

    async Task IBaseDbContext.WrapInTransaction(Func<Task> action, CancellationToken cancellationToken)
    {
        var strategy = Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async ct =>
        {
            using var transaction = await Database.BeginTransactionAsync(ct);

            try
            {
                await action.Invoke();
                await transaction.CommitAsync(ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }
}