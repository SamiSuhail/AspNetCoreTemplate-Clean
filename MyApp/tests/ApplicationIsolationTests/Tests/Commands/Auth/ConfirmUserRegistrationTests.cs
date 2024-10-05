using MyApp.Application.Interfaces.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.UserConfirmation.Failures;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class ConfirmUserRegistrationTests(AppFactory appFactory) : BaseTest(appFactory), IAsyncLifetime
{
    private UserEntity _user = default!;
    private ConfirmUserRegistrationRequest _request = default!;
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await ArrangeDbContext.ArrangeRandomUnconfirmedUser();
        _request = new(_user.UserConfirmation!.Code);
    }

    [Fact]
    public async Task GivenHappyPath_ThenUserIsConfirmed()
    {
        // Act
        var response = await UnauthorizedAppClient.ConfirmUserRegistration(_request);

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(_user.Id);
        using var _ = new AssertionScope();
        user.IsEmailConfirmed.Should().BeTrue();
        user.UserConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenUserIsAuthenticated_ReturnsAnonymousOnlyError()
    {
        // Act
        var response = await AppClient.ConfirmUserRegistration(_request);

        // Assert
        response.AssertAnonymousOnlyError();
        await AssertNotConfirmed();
    }

    [Fact]
    public async Task GivenUserAlreadyConfirmed_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.ConfirmUser(_user);

        // Act
        var response = await UnauthorizedAppClient.ConfirmUserRegistration(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Fact]
    public async Task GivenCodeDoesNotExist_ReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with { Code = "000000" };

        // Act
        var response = await UnauthorizedAppClient.ConfirmUserRegistration(request);

        // Assert
        AssertInvalidFailure(response);
        await AssertNotConfirmed();
    }

    [Fact]
    public async Task GivenCodeIsExpired_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.ArrangeExpireAllConfirmations(_user.Id);

        // Act
        var response = await UnauthorizedAppClient.ConfirmUserRegistration(_request);

        // Assert
        AssertInvalidFailure(response);
        await AssertNotConfirmed();
    }

    private async Task AssertNotConfirmed()
    {
        var user = await AssertDbContext.GetUser(_user.Id);
        using var _ = new AssertionScope();
        user.IsEmailConfirmed.Should().BeFalse();
        user.UserConfirmation.Should().NotBeNull();
    }

    private static void AssertInvalidFailure(IApiResponse response)
    {
        response.AssertSingleBadRequestError(UserConfirmationCodeInvalidFailure.Key, UserConfirmationCodeInvalidFailure.Message);
    }
}
