using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Domain.Infra.Instance.Failures;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;

namespace MyApp.Tests.Integration.Tests.Features.Instance;

public partial class WhenForgotPasswordInstanceTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private ForgotPasswordRequest _request = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _request = new(User.Entity.Email, User.Entity.Username);
    }

    [Fact]
    public async Task GivenInstanceExists_ReturnsSuccess()
    {
        // Arrange
        var instanceName = await ArrangeDbContext.ArrangeRandomInstance();
        await ArrangeDbContext.UpdateUserInstance(User.Entity.Id, instanceName);

        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(_request, instanceName);

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
        var response = await UnauthorizedAppClient.ForgotPassword(_request);

        // Assert
        response.AssertSingleBadRequestError(ForgotPasswordUserNotFoundFailure.Key, ForgotPasswordUserNotFoundFailure.Message);
    }

    [Fact]
    public async Task GivenInstanceNotExists_ReturnsInvalidFailure()
    {
        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(_request, RandomData.InstanceName);

        // Assert
        response.AssertSingleBadRequestError(InstanceNotFoundFailure.Key, InstanceNotFoundFailure.Message);
    }
}