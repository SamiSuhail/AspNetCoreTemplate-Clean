using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.Register;
using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Tests.Integration.Tests.Commands.Auth;

public class RegisterTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private readonly RegisterRequest _request = RandomRequests.Register;

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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenUsernameTaken_ReturnsUsernameConflictFailure(bool existingUserIsConfirmed)
    {
        // Arrange
        var _ = existingUserIsConfirmed
            ? await ArrangeDbContext.ArrangeConfirmedUser(_request.Username, RandomData.Password, RandomData.Email)
            : await ArrangeDbContext.ArrangeUnconfirmedUser(_request.Username, RandomData.Password, RandomData.Email);

        // Act
        var response = await UnauthorizedAppClient.Register(_request);

        // Assert
        response.AssertSingleBadRequestError(UserConflictFailure.UsernameTakenKey, UserConflictFailure.UsernameTakenMessage);
        await AssertUserNotExists(_request.Username, _request.Email);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenEmailTaken_ReturnsEmailConflictFailure(bool existingUserIsConfirmed)
    {
        // Arrange
        var _ = existingUserIsConfirmed
            ? await ArrangeDbContext.ArrangeConfirmedUser(RandomData.Username, RandomData.Password, _request.Email)
            : await ArrangeDbContext.ArrangeUnconfirmedUser(RandomData.Username, RandomData.Password, _request.Email);

        // Act
        var response = await UnauthorizedAppClient.Register(_request);

        // Assert
        response.AssertSingleBadRequestError(UserConflictFailure.EmailTakenKey, UserConflictFailure.EmailTakenMessage);
        await AssertUserNotExists(_request.Username, _request.Email);
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
