using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Access.Scope;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Infra.Instance;
using MyApp.Infrastructure.Auth;
using MyApp.Infrastructure.Database.EFCore;
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
        await SeedInstanceWriteScope(adminUserId);
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
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        return newInstance.Id;
    }

    private async Task<int> SeedAdminUser(int defaultInstanceId)
    {
        var existingUser = await _dbContext.Set<UserEntity>()
            .Where(u => u.Username == _adminUserSettings.Username)
            .FirstOrDefaultAsync();

        if (existingUser == null)
        {
            var newUser = UserEntity.CreateConfirmed(defaultInstanceId, _adminUserSettings.Username, _adminUserSettings.Password, "fake@email.com");
            _dbContext.Add(newUser);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            return newUser.Id;
        }

        if (!_adminUserSettings.Password.Verify(existingUser.PasswordHash))
        {
            existingUser.UpdatePassword(_adminUserSettings.Password);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        return existingUser.Id;
    }

    private async Task SeedInstanceWriteScope(int adminUserId)
    {
        var existingScope = await _dbContext.Set<ScopeEntity>()
            .Include(s => s.ScopeUsers.Where(su => su.UserId == adminUserId))
            .Where(s => s.Name == CustomScopes.InstanceWrite)
            .Select(s => new
            {
                s.Id,
                s.ScopeUsers,
            })
            .FirstOrDefaultAsync();

        if (existingScope?.ScopeUsers.Count > 0)
            return;

        if (existingScope == null)
        {
            var newScope = ScopeEntity.Create(CustomScopes.InstanceWrite);
            var newUserScope = UserScopeEntity.Create(adminUserId, newScope);
            _dbContext.Add(newScope);
            _dbContext.Add(newUserScope);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            return;
        }

        var userScope = UserScopeEntity.Create(adminUserId, existingScope.Id);
        _dbContext.Add(userScope);
        await _dbContext.SaveChangesAsync(CancellationToken.None);
    }
}
