namespace MyApp.Server.Infrastructure.Abstractions.Database;

public interface IAppDbContextFactory : IDisposable
{
    IScopedDbContext CreateScopedDbContext();
    Task<IScopedDbContext> CreateScopedDbContextAsync(CancellationToken cancellationToken);
    ITransientDbContext CreateTransientDbContext();
    Task<ITransientDbContext> CreateTransientDbContextAsync(CancellationToken cancellationToken);
}
