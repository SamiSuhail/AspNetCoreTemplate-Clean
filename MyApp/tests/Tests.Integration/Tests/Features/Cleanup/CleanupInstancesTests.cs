using MyApp.Application.Infrastructure.Repositories;
using MyApp.Application.Modules.BackgroundJobs.Cleanup.Instances;
using MyApp.Domain.Infra.Instance;

namespace MyApp.Tests.Integration.Tests.Features.Cleanup;

public class CleanupInstancesTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ThenInstanceIsDeleted()
    {
        // Arrange
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance(isCleanupEnabled: true);
        await ArrangeDbContext.ArrangeExpireEntities<InstanceEntity>(x => x.Name == instanceName);

        // Act
        await ScopedServices.GetRequiredService<ISender>()
            .Send(new CleanupInstancesRequest());

        // Assert
        var instanceIsDeleted = await AssertDbContext.Set<InstanceEntity>()
            .NoneAsync(x => x.Name == instanceName);

        instanceIsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GivenInstanceNotExpired_ThenInstanceIsNotDeleted()
    {
        // Arrange
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance(isCleanupEnabled: true);

        // Act
        await ScopedServices.GetRequiredService<ISender>()
            .Send(new CleanupInstancesRequest());

        // Assert
        var instanceIsDeleted = await AssertDbContext.Set<InstanceEntity>()
            .AnyAsync(x => x.Name == instanceName);

        instanceIsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GivenCleanupNotEnabled_ThenInstanceIsNotDeleted()
    {
        // Arrange
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance(isCleanupEnabled: false);
        await ArrangeDbContext.ArrangeExpireEntities<InstanceEntity>(x => x.Name == instanceName);

        // Act
        await ScopedServices.GetRequiredService<ISender>()
            .Send(new CleanupInstancesRequest());

        // Assert
        var instanceIsDeleted = await AssertDbContext.Set<InstanceEntity>()
            .AnyAsync(x => x.Name == instanceName);

        instanceIsDeleted.Should().BeTrue();
    }
}
