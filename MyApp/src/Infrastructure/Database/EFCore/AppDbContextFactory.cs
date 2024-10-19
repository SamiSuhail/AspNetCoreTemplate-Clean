﻿using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Database;

namespace MyApp.Infrastructure.Database.EFCore;

public sealed class AppDbContextFactory(IDbContextFactory<AppDbContext> factory) : IAppDbContextFactory
{
    private readonly List<AppDbContext> _transientDbContexts = [];
    private AppDbContext? _scopedDbContext;

    public IScopedDbContext CreateScopedDbContext()
    {
        _scopedDbContext ??= factory.CreateDbContext();
        return _scopedDbContext;
    }

    public async Task<IScopedDbContext> CreateScopedDbContextAsync(CancellationToken cancellationToken)
    {
        _scopedDbContext ??= await factory.CreateDbContextAsync(cancellationToken);
        return _scopedDbContext;
    }

    public ITransientDbContext CreateTransientDbContext()
    {
        var context = factory.CreateDbContext();
        _transientDbContexts.Add(context);
        return context;
    }

    public async Task<ITransientDbContext> CreateTransientDbContextAsync(CancellationToken cancellationToken)
    {
        var context = await factory.CreateDbContextAsync(cancellationToken);
        _transientDbContexts.Add(context);
        return context;
    }

    public void Dispose()
    {
        _scopedDbContext?.Dispose();
        foreach (var context in _transientDbContexts)
        {
            context?.Dispose();
        }
    }
}
