using MyApp.Domain.Access.Scope;
using MyApp.Domain.Infra.Instance.Failures;

namespace MyApp.Tests.Integration.Tests.Commands.Infra;

public class CreateInstanceTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ThenCreatesInstance()
    {
        // Act
        var response = await AppClient.CreateInstance(RandomRequests.CreateInstance);

        // Assert
        response.AssertSuccess();
    }

    [Fact]
    public async Task GivenInstanceAlreadyExists_ReturnsInvalidFailure()
    {
        // Arrange
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance();
        var request = RandomRequests.CreateInstance with
        {
            Name = instanceName,
        };

        // Act
        var response = await AppClient.CreateInstance(request);

        // Assert
        response.AssertSingleBadRequestError(InstanceNameTakenFailure.Key, InstanceNameTakenFailure.Message);
    }
}
