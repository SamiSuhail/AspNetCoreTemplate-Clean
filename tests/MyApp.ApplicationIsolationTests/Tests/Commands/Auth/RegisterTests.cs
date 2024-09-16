using MyApp.Server.Application.Commands.Auth.Registration;
using MyApp.Server.Application.Commands.Auth.Registration.Register;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class RegisterTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private readonly RegisterRequest _request = new(RandomData.Email, RandomData.Username, RandomData.Password);

    [Fact]
    public async Task GivenHappyPath_ThenCreatesUnconfirmedUser()
    {
        // Act
        var response = await UnauthorizedAppClient.Register(_request);

        // Assert
        response.AssertSuccess();
        var user = await GetUser(_request.Username, _request.Email);
        user.Should().NotBeNull();

        using (new AssertionScope())
        {
            user!.IsEmailConfirmed.Should().BeFalse();
            user.CreatedAt.ShouldBeNow();
            _request.Password.Verify(user.PasswordHash).Should().BeTrue();
        }
        user.UserConfirmation.Should().NotBeNull();
        using (new AssertionScope())
        {
            user.UserConfirmation!.CreatedAt.ShouldBeNow();
            user.UserConfirmation.Code.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task GivenHappyPath_ThenProducesMessage()
    {
        // Act
        var response = await UnauthorizedAppClient.Register(_request);

        // Assert
        response.AssertSuccess();
        AssertMessage.Produced<SendUserConfirmationMessage>();
    }

    [Fact]
    public async Task GivenDatabaseError_ThenNoMessageProduced()
    {
        // Arrange
        var request = _request with
        {
            Email = TestUser.Email
        };

        // Act
        var response = await UnauthorizedAppClient.Register(request);

        // Assert
        response.AssertBadRequest();
        AssertMessage.Produced<SendUserConfirmationMessage>(Times.Never());
        await AssertUserNotExists(request.Username, request.Email);
    }

    [Fact]
    public async Task GivenMessageProducerThrows_ThenNoDataIsPersisted()
    {
        // Arrange
        MockBag.Get<IMessageProducer>()
            .Setup(m => m.Send(It.IsAny<SendUserConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var response = await UnauthorizedAppClient.Register(_request);

        // Assert
        response.AssertInternalServerError();
        await AssertUserNotExists(_request.Username, _request.Email);
    }

    [Fact]
    public async Task GivenUserIsAuthenticated_ReturnsAnonymousOnlyError()
    {
        // Act
        var response = await AppClient.Register(_request);

        // Assert
        response.AssertAnonymousOnlyError();
        await AssertUserNotExists(_request.Username, _request.Email);
    }

    [Fact]
    public async Task GivenUsernameTaken_ReturnsUsernameConflictFailure()
    {
        // Arrange
        var request = _request with
        {
            Username = TestUser.Username,
        };

        // Act
        var response = await UnauthorizedAppClient.Register(request);

        // Assert
        response.AssertSingleBadRequestError(RegisterConflictFailure.UsernameTakenKey, RegisterConflictFailure.UsernameTakenMessage);
        await AssertUserNotExists(request.Username, request.Email);
    }

    [Fact]
    public async Task GivenEmailTaken_ReturnsEmailConflictFailure()
    {
        // Arrange
        var request = _request with
        {
            Email = TestUser.Email,
        };

        // Act
        var response = await UnauthorizedAppClient.Register(request);

        // Assert
        response.AssertSingleBadRequestError(RegisterConflictFailure.EmailTakenKey, RegisterConflictFailure.EmailTakenMessage);
        await AssertUserNotExists(request.Username, request.Email);
    }

    private async Task AssertUserNotExists(string username, string email)
    {
        var user = await GetUser(username, email);
        user.Should().BeNull();
    }

    private async Task<UserEntity?> GetUser(string username, string email)
    {
        return await AssertDbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Include(x => x.UserConfirmation)
            .FirstOrDefaultAsync(x => x.Username == username && x.Email == email);
    }
}
