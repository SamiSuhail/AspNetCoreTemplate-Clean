using MyApp.Application.Interfaces.Commands.Auth.Registration.Register;
using MyApp.Domain.Infra.Instance.Failures;

namespace MyApp.Tests.Integration.Tests.Features.Instance;

public class WhenRegisterInstanceTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenInstanceExists_ReturnsSuccess()
    {
        // Arrange
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance();

        // Act
        var response = await UnauthorizedAppClient.Register(RandomRequests.Register, instanceName);

        // Assert
        response.AssertSuccess();
    }

    [Fact]
    public async Task GivenUserExistsForOtherInstance_ThenNoConflict()
    {
        // Arrange
        var user = await ArrangeDbContext.ArrangeRandomConfirmedUser();
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance();

        var request = new RegisterRequest(user.Email, user.Username, RandomData.Password);

        // Act
        var response = await UnauthorizedAppClient.Register(request, instanceName);

        // Assert
        response.AssertSuccess();
    }

    [Fact]
    public async Task GivenInstanceNotExists_ReturnsInvalidFailure()
    {
        // Act
        var response = await UnauthorizedAppClient.Register(RandomRequests.Register, RandomData.InstanceName);

        // Assert
        response.AssertSingleBadRequestError(InstanceNotFoundFailure.Key, InstanceNotFoundFailure.Message);
    }
}
