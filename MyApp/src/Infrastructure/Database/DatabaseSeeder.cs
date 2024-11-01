using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Access.Scope;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Infra.Instance;
using MyApp.Infrastructure.Auth;
using MyApp.Infrastructure.Database.EFCore;
using MyApp.Utilities.Collections;
using static MyApp.Domain.Infra.Instance.InstanceConstants;

namespace MyApp.Infrastructure.Database;

public interface IDatabaseSeeder
{
    Task SeedDatabase();
}

public class DatabaseSeeder(
    IDbContextFactory<AppDbContext> dbFactory,
    AuthSettings authSettings
    ) : IDatabaseSeeder
{
    private IBaseDbContext _dbContext = default!;
    private readonly IDbContextFactory<AppDbContext> _dbFactory = dbFactory;
    private readonly AdminUserSettings _adminUserSettings = authSettings.AdminUser;

    public async Task SeedDatabase()
    {
        await using var dbContext = _dbContext = await _dbFactory.CreateDbContextAsync();

        var defaultInstanceId = await SeedDefaultInstance();
        var adminUserId = await SeedAdminUser(defaultInstanceId);
        await SeedScopes(adminUserId);
    }

    private async Task<int> SeedDefaultInstance()
    {
        var existingInstance = await _dbContext.Set<InstanceEntity>()
            .Where(i => i.Name == DefaultInstanceName)
            .Select(i => new
            {
                i.Id,
            })
            .FirstOrDefaultAsync();

        if (existingInstance != null)
            return existingInstance.Id;

        var newInstance = InstanceEntity.Create(DefaultInstanceName, isCleanupEnabled: false);
        _dbContext.Add(newInstance);
        await _dbContext.SaveChangesAsync();
        return newInstance.Id;
    }

    private async Task<int> SeedAdminUser(int defaultInstanceId)
    {
        var existingUser = await _dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Where(u => u.Username == _adminUserSettings.Username && u.InstanceId == defaultInstanceId)
            .FirstOrDefaultAsync();

        if (existingUser == null)
        {
            var newUser = UserEntity.CreateConfirmed(defaultInstanceId, _adminUserSettings.Username, _adminUserSettings.Password, _adminUserSettings.Email);
            _dbContext.Add(newUser);
            await _dbContext.SaveChangesAsync();
            return newUser.Id;
        }

        if (!_adminUserSettings.Password.Verify(existingUser.PasswordHash))
        {
            existingUser.UpdatePassword(_adminUserSettings.Password);
            await _dbContext.SaveChangesAsync();
        }

        return existingUser.Id;
    }

    private async Task SeedScopes(int adminUserId)
    {
        string[] scopeNames = CustomScopes.All;
        object[] alreadySeededScopes = await GetExistingScopes(adminUserId, scopeNames);

        if (alreadySeededScopes.Length == scopeNames.Length)
            return;

        var scopeNamesToSeed = scopeNames.Where(sn => alreadySeededScopes.NotContains(sn));
        foreach (var scopeName in scopeNamesToSeed)
        {
            var newScope = ScopeEntity.Create(scopeName);
            var newUserScope = UserScopeEntity.Create(adminUserId, newScope);
            _dbContext.Add(newScope);
            _dbContext.Add(newUserScope);
        }

        await _dbContext.SaveChangesAsync();

        async Task<string[]> GetExistingScopes(int adminUserId, string[] scopeNames)
        {
            return await _dbContext.Set<ScopeEntity>()
                        .Where(s => scopeNames.Contains(s.Name))
                        .Select(s => s.Name)
                        .ToArrayAsync();
        }
    }
}
