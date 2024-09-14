using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.EmailConfirmation.Failures;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Modules.Commands.Auth.Registration.ConfirmEmail;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class ConfirmEmailTests(AppFactory appFactory) : BaseTest(appFactory), IAsyncLifetime
{
    private UserEntity _user = null!;
    private ConfirmEmailRequest _request = null!;
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await ArrangeDbContext.ArrangeRandomUser();
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
    public async Task GivenUserIsAuthenticated_ThenReturnsAnonymousOnlyError()
    {
        // Act
        var response = await AppClient.ConfirmEmail(_request);

        // Assert
        response.AssertAnonymousOnlyError();
        await AssertNotConfirmed();
    }

    [Fact]
    public async Task GivenUserAlreadyConfirmed_ThenReturnsInvalidFailure()
    {
        // Arrange
        _user.ConfirmEmail();
        ArrangeDbContext.Remove(_user.EmailConfirmation!);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var response = await UnauthorizedAppClient.ConfirmEmail(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Fact]
    public async Task GivenCodeDoesNotExist_ThenReturnsInvalidFailure()
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
    public async Task GivenCodeIsExpired_ThenReturnsInvalidFailure()
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
