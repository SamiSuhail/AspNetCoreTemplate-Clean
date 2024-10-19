using MyApp.Domain.Auth.User;
using MyApp.Domain.Infra.Instance;

namespace MyApp.Tests.Integration.Utilities.Arrange;

public static class DbContextInstanceExtensions
{
    public static async Task<string> ArrangeRandomInstance(this IBaseDbContext dbContext, bool isCleanupEnabled = false)
    {
        var instanceName = RandomData.InstanceName;
        var instance = InstanceEntity.Create(instanceName, isCleanupEnabled);
        dbContext.Add(instance);
        await dbContext.SaveChangesAsync();
        return instanceName;
    }

    public static async Task UpdateUserInstance(this IBaseDbContext dbContext, int userId, string instanceName)
    {
        var instance = await dbContext.Set<InstanceEntity>()
            .FirstAsync(i => i.Name == instanceName);

        var rowsUpdated = await dbContext.Set<UserEntity>()
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.InstanceId, instance.Id));

        rowsUpdated.Should().Be(1);
    }
}
