using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.Infra.Instance.Failures;

namespace MyApp.Tests.Integration.Tests.Features.Instance;

public partial class WhenLoginInstanceTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private LoginRequest _request = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _request = new(User.Entity.Username, User.Password, []);
    }

    [Fact]
    public async Task GivenInstanceExists_ReturnsSuccess()
    {
        // Arrange
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance();
        await ArrangeDbContext.UpdateUserInstance(User.Entity.Id, instanceName);

        // Act
        var response = await UnauthorizedAppClient.Login(_request, instanceName);

        // Assert
        response.AssertSuccess();
    }

    [Fact]
    public async Task GivenUserExistsForOtherInstance_ReturnsInvalidFailure()
    {
        // Arrange
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance();
        await ArrangeDbContext.UpdateUserInstance(User.Entity.Id, instanceName);

        // Act
        var response = await UnauthorizedAppClient.Login(_request);

        // Assert
        response.AssertSingleBadRequestError(LoginInvalidFailure.Key, LoginInvalidFailure.Message);
    }

    [Fact]
    public async Task GivenInstanceNotExists_ReturnsInvalidFailure()
    {
        // Act
        var response = await UnauthorizedAppClient.Login(_request, RandomData.InstanceName);

        // Assert
        response.AssertSingleBadRequestError(InstanceNotFoundFailure.Key, InstanceNotFoundFailure.Message);
    }
}