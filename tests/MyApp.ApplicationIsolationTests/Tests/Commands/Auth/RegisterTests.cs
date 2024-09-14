using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Modules.Commands.Auth.Registration.Register;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class RegisterTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private readonly RegisterRequest _request = new(RandomData.Email, RandomData.Username, RandomData.Password);

    [Fact]
    public async Task GivenUsernameAndEmailNotTaken_ThenCreatesUnconfirmedUser()
    {
        // Act
        var response = await UnauthorizedAppClient.Register(_request);

        // Assert
        response.AssertSuccess();
        var user = await GetUser(_request.Username, _request.Email);
        user.Should().NotBeNull();

        using (new AssertionScope())
        {
            user!.Id.Should().NotBe(0);
            user.IsEmailConfirmed.Should().BeFalse();
            user.CreatedAt.ShouldBeNow();
            _request.Password.Verify(user.PasswordHash).Should().BeTrue();
        }
        user.EmailConfirmation.Should().NotBeNull();
        using (new AssertionScope())
        {
            user.EmailConfirmation!.Id.Should().NotBe(0);
            user.EmailConfirmation.CreatedAt.ShouldBeNow();
            user.EmailConfirmation.Code.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task GivenUsernameTaken_ThenReturnsBadRequest()
    {
        // Arrange
        var request = _request with
        {
            Username = TestUser.Username,
        };

        // Act
        var response = await UnauthorizedAppClient.Register(request);

        // Assert
        var user = await GetUser(request.Username, request.Email);
        user.Should().BeNull();
        response.AssertSingleBadRequestError(RegisterConflictFailure.UsernameTakenKey, RegisterConflictFailure.UsernameTakenMessage);
    }

    [Fact]
    public async Task GivenEmailTaken_ThenReturnsBadRequest()
    {
        // Arrange
        var request = _request with
        {
            Email = TestUser.Email,
        };

        // Act
        var response = await UnauthorizedAppClient.Register(request);

        // Assert
        var user = await GetUser(request.Username, request.Email);
        user.Should().BeNull();
        response.AssertSingleBadRequestError(RegisterConflictFailure.EmailTakenKey, RegisterConflictFailure.EmailTakenMessage);
    }

    private async Task<UserEntity?> GetUser(string username, string email)
    {
        return await AssertDbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Include(x => x.EmailConfirmation)
            .FirstOrDefaultAsync(x => x.Username == username && x.Email == email);
    }
}
