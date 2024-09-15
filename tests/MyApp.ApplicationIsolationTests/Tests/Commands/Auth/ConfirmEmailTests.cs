using MyApp.Server.Application.Commands.Auth.Registration.ConfirmEmail;
using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.EmailConfirmation.Failures;
using MyApp.Server.Domain.Auth.User;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class ConfirmEmailTests(AppFactory appFactory) : BaseTest(appFactory), IAsyncLifetime
{
    private UserEntity _user = default!;
    private ConfirmEmailRequest _request = default!;
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await ArrangeDbContext.ArrangeRandomUnconfirmedUser();
        _request = new(_user.EmailConfirmation!.Code);
    }

    [Fact]
    public async Task GivenHappyPath_ThenUserIsConfirmed()
    {
        // Act
        var response = await UnauthorizedAppClient.ConfirmEmail(_request);

        // Assert
        response.AssertSuccess();
        var user = await GetUser();
        using var _ = new AssertionScope();
        user.IsEmailConfirmed.Should().BeTrue();
        user.EmailConfirmation.Should().BeNull();
    }

    [Fact]
    public async Task GivenUserIsAuthenticated_ReturnsAnonymousOnlyError()
    {
        // Act
        var response = await AppClient.ConfirmEmail(_request);

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
        var response = await UnauthorizedAppClient.ConfirmEmail(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Fact]
    public async Task GivenCodeDoesNotExist_ReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with { Code = "000000" };

        // Act
        var response = await UnauthorizedAppClient.ConfirmEmail(request);

        // Assert
        AssertInvalidFailure(response);
        await AssertNotConfirmed();
    }

    [Fact]
    public async Task GivenCodeIsExpired_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.Set<EmailConfirmationEntity>()
            .Where(ec => ec.Id == _user.EmailConfirmation!.Id)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(
                    ec => ec.CreatedAt,
                    DateTime.UtcNow.AddMinutes(-EmailConfirmationConstants.ExpirationTimeMinutes - 1)));

        // Act
        var response = await UnauthorizedAppClient.ConfirmEmail(_request);

        // Assert
        AssertInvalidFailure(response);
        await AssertNotConfirmed();
    }

    private async Task<UserEntity> GetUser()
    {
        var user = await AssertDbContext.GetUser(_user.Id);
        user.Should().NotBeNull();
        return user!;
    }

    private async Task AssertNotConfirmed()
    {
        var user = await GetUser();
        using var _ = new AssertionScope();
        user.IsEmailConfirmed.Should().BeFalse();
        user.EmailConfirmation.Should().NotBeNull();
    }

    private static void AssertInvalidFailure(IApiResponse response)
    {
        response.AssertSingleBadRequestError(EmailConfirmationCodeInvalidFailure.Key, EmailConfirmationCodeInvalidFailure.Message);
    }
}
