using MyApp.Application.Commands.Auth.Registration;
using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Auth.UserConfirmation.Failures;
using MyApp.Domain.Auth.User;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Interfaces.Commands.Auth.Registration.ResendConfirmation;
using MyApp.Tests.Utilities.Clients.Extensions;

namespace MyApp.Tests.Integration.Tests.Commands.Auth;

public class ResendConfirmationTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private UserEntity _user = default!;
    private ResendConfirmationRequest _request = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await ArrangeDbContext.ArrangeRandomUnconfirmedUser();
        _request = new ResendConfirmationRequest(_user.Email);
    }

    [Fact]
    public async Task GivenHappyPath_ThenRenewsConfirmationCode()
    {
        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(_request);

        // Assert
        response.AssertSuccess();
        var userConfirmation = await GetUserConfirmation();
        userConfirmation.Should().NotBeNull();
        using var _ = new AssertionScope();
        userConfirmation!.Id.Should().NotBe(_user.UserConfirmation!.Id);
        userConfirmation!.Code.Should().NotBe(_user.UserConfirmation!.Code);
        userConfirmation!.CreatedAt.ShouldBeNow();
    }

    [Fact]
    public async Task GivenHappyPath_ThenProducesMessage()
    {
        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(_request);

        // Assert
        response.AssertSuccess();
        MockBag.AssertProduced<SendUserConfirmationMessage>();
    }

    [Fact]
    public async Task GivenDatabaseError_ThenNoMessageProduced()
    {
        // Arrange
        var request = _request with
        {
            Email = RandomData.Email,
        };

        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(request);

        // Assert
        response.AssertBadRequest();
        MockBag.AssertProduced<SendUserConfirmationMessage>(Times.Never());
        await AssertUserConfirmationUnchanged();
    }

    [Fact]
    public async Task GivenMessageProducerThrows_ThenNoDataIsPersisted()
    {
        // Arrange
        MockBag.Get<IMessageProducer>()
            .Setup(m => m.Send(It.IsAny<SendUserConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(_request);

        // Assert
        response.AssertInternalServerError();
        await AssertUserConfirmationUnchanged();
    }

    [Fact]
    public async Task GivenUserIsAuthenticated_ReturnsAnonymousOnlyError()
    {
        // Act
        var response = await AppClient.ResendConfirmation(_request);

        // Assert
        response.AssertAnonymousOnlyError();
        await AssertUserConfirmationUnchanged();
    }

    [Fact]
    public async Task GivenEmailDoesNotExist_ReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with { Email = RandomData.Email };

        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(request);

        // Assert
        AssertInvalidFailure(response);
        await AssertUserConfirmationUnchanged();
    }

    [Fact]
    public async Task GivenUserAlreadyConfirmed_ReturnsInvalidFailure()
    {
        // Arrange
        await ArrangeDbContext.ConfirmUser(_user);

        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(_request);

        // Assert
        AssertInvalidFailure(response);
        await AssertUserConfirmationUnchanged();
    }

    private async Task<UserConfirmationEntity> GetUserConfirmation()
    {
        var user = await AssertDbContext.GetUser(_user.Id);
        return user!.UserConfirmation!;
    }

    private async Task AssertUserConfirmationUnchanged()
    {
        var userConfirmation = await GetUserConfirmation();

        if (_user.UserConfirmation == null)
        {
            userConfirmation.Should().BeNull();
            return;
        }

        userConfirmation.Should().NotBeNull();
        userConfirmation.Id.Should().Be(_user.UserConfirmation!.Id);
        userConfirmation.Code.Should().Be(_user.UserConfirmation.Code);
    }

    private static void AssertInvalidFailure(IApiResponse response)
    {
        response.AssertSingleBadRequestError(ResendConfirmationInvalidFailure.Key, ResendConfirmationInvalidFailure.Message);
    }
}
