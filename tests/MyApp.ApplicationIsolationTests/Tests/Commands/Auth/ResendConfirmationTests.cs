
using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.EmailConfirmation.Failures;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Messaging;
using MyApp.Server.Modules.Commands.Auth.Registration;
using MyApp.Server.Modules.Commands.Auth.Registration.ResendConfirmation;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class ResendConfirmationTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private UserEntity _user = null!;
    private ResendConfirmationRequest _request = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await ArrangeDbContext.ArrangeRandomUser();
        _request = new ResendConfirmationRequest(_user.Email);
    }

    [Fact]
    public async Task GivenHappyPath_ThenRenewsConfirmationCode()
    {
        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(_request);

        // Assert
        response.AssertSuccess();
        var emailConfirmation = await GetEmailConfirmation();
        emailConfirmation.Should().NotBeNull();
        using var _ = new AssertionScope();
        emailConfirmation!.Id.Should().NotBe(_user.EmailConfirmation!.Id);
        emailConfirmation!.Code.Should().NotBe(_user.EmailConfirmation!.Code);
        emailConfirmation!.CreatedAt.ShouldBeNow();
    }

    [Fact]
    public async Task GivenHappyPath_ThenProducesMessage()
    {
        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(_request);

        // Assert
        response.AssertSuccess();
        AssertHelper.AssertMessageProduced<SendEmailConfirmationMessage>();
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
        AssertHelper.AssertMessageProduced<SendEmailConfirmationMessage>(Times.Never());
        await AssertEmailConfirmationUnchanged();
    }

    [Fact]
    public async Task GivenMessageProducerThrows_ThenNoDataIsPersisted()
    {
        // Arrange
        MockBag.Get<IMessageProducer>()
            .Setup(m => m.Send(It.IsAny<SendEmailConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(_request);

        // Assert
        response.AssertInternalServerError();
        await AssertEmailConfirmationUnchanged();
    }

    [Fact]
    public async Task GivenUserIsAuthenticated_ThenReturnsAnonymousOnlyError()
    {
        // Act
        var response = await AppClient.ResendConfirmation(_request);

        // Assert
        response.AssertAnonymousOnlyError();
        await AssertEmailConfirmationUnchanged();
    }

    [Fact]
    public async Task GivenEmailDoesNotExist_ThenReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with { Email = RandomData.Email };

        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(request);

        // Assert
        AssertInvalidFailure(response);
        await AssertEmailConfirmationUnchanged();
    }

    [Fact]
    public async Task GivenUserAlreadyConfirmed_ThenReturnsInvalidFailure()
    {
        // Arrange
        _user.ConfirmEmail();
        ArrangeDbContext.Remove(_user.EmailConfirmation!);
        await ArrangeDbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var response = await UnauthorizedAppClient.ResendConfirmation(_request);

        // Assert
        AssertInvalidFailure(response);
        var emailConfirmation = await GetEmailConfirmation();
        emailConfirmation.Should().BeNull();
    }

    private async Task<EmailConfirmationEntity> GetEmailConfirmation()
    {
        var user = await AssertDbContext.GetUser(_user.Id);
        user.Should().NotBeNull();
        return user!.EmailConfirmation!;
    }

    private async Task AssertEmailConfirmationUnchanged()
    {
        var emailConfirmation = await GetEmailConfirmation();
        emailConfirmation.Should().NotBeNull();
        emailConfirmation.Id.Should().Be(_user.EmailConfirmation!.Id);
        emailConfirmation.Code.Should().Be(_user.EmailConfirmation.Code);
    }

    private static void AssertInvalidFailure(IApiResponse response)
    {
        response.AssertSingleBadRequestError(ResendConfirmationInvalidFailure.Key, ResendConfirmationInvalidFailure.Message);
    }
}
