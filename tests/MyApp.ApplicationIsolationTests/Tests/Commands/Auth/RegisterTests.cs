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
        MockBag.AssertProduced<SendUserConfirmationMessage>();
    }

    [Fact]
    public async Task GivenDatabaseError_ThenNoMessageProduced()
    {
        // Arrange
        var request = _request with
        {
            Email = User.Entity.Email
        };

        // Act
        var response = await UnauthorizedAppClient.Register(request);

        // Assert
        response.AssertBadRequest();
        MockBag.AssertProduced<SendUserConfirmationMessage>(Times.Never());
        await AssertUserNotExists(request.Username, request.Email);
    }

    [Fact]
    public async Task GivenMessageProducerThrows_ThenNoDataIsPersisted()
    {
        // Arrange
        MockBag.ArrangeMessageThrows<SendUserConfirmationMessage>();

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
            Username = User.Entity.Username,
        };

        // Act
        var response = await UnauthorizedAppClient.Register(request);

        // Assert
        response.AssertSingleBadRequestError(UserConflictFailure.UsernameTakenKey, UserConflictFailure.UsernameTakenMessage);
        await AssertUserNotExists(request.Username, request.Email);
    }

    [Fact]
    public async Task GivenEmailTaken_ReturnsEmailConflictFailure()
    {
        // Arrange
        var request = _request with
        {
            Email = User.Entity.Email,
        };

        // Act
        var response = await UnauthorizedAppClient.Register(request);

        // Assert
        response.AssertSingleBadRequestError(UserConflictFailure.EmailTakenKey, UserConflictFailure.EmailTakenMessage);
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
