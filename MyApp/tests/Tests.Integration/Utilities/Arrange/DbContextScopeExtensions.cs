using MyApp.Domain.Access.Scope;
using MyApp.Utilities.Collections;

namespace MyApp.Tests.Integration.Utilities.Arrange;

public static class DbContextScopeExtensions
{
    public static async Task<string> ArrangeRandomScope(this IBaseDbContext dbContext, int userId)
    {
        var user = await dbContext.GetUser(userId);
        var scopeName = RandomData.ScopeName;
        user.AddScopes([ScopeEntity.Create(scopeName)]);
        await dbContext.SaveChangesAsync();
        return scopeName;
    }

    public static async Task ArrangeScopesForUser(this IBaseDbContext dbContext, string[] scopeNames, int userId)
    {
        var existingScopes = await dbContext.Set<ScopeEntity>()
            .Where(x => scopeNames.Contains(x.Name))
            .ToArrayAsync();

        var allScopes = scopeNames.Where(scopeName => existingScopes.None(s => s.Name == scopeName))
            .Select(ScopeEntity.Create)
            .Concat(existingScopes)
            .ToArray();

        var user = await dbContext.GetUser(userId);
        user.AddScopes(allScopes);
        await dbContext.SaveChangesAsync();
    }
}
