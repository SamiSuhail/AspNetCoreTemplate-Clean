using MyApp.Application.Infrastructure;
using MyApp.Application.Infrastructure.BackgroundJobs.Cleanup.Instances;
using MyApp.Domain.Infra.Instance;

namespace MyApp.Tests.Integration.Tests.Features.Cleanup;

public class CleanupInstancesTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ThenInstancesAreDeleted()
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
}
