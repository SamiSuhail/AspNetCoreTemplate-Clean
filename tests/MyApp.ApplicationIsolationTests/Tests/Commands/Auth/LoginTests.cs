using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Modules.Commands.Auth.Login;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class LoginTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private readonly LoginRequest _request = new(TestUser.Username, TestUser.Password);

    [Fact]
    public async Task GivenHappyPath_ReturnsValidAccessToken()
    {
        // Act
        var response = await UnauthorizedAppClient.Login(_request);

        // Assert
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        response.Content!.AccessToken.Should().NotBeNullOrEmpty();

        var jwtReader = ScopedServices.GetRequiredService<IJwtReader>();
        var user = jwtReader.ReadAccessToken(response.Content.AccessToken);
        using var _ = new AssertionScope();
        user.Id.Should().Be(User.Id);
        user.Username.Should().Be(User.Username);
        user.Email.Should().Be(User.Email);
    }

    [Fact]
    public async Task GivenEmailNotConfirmed_ReturnsEmailNotConfirmedFailure()
    {
        // Arrange
        var password = RandomData.Password;
        var user = await ArrangeDbContext.ArrangeUnconfirmedUser(RandomData.Username, password, RandomData.Email);
        var request = _request with
        {
            Username = user.Username,
            Password = password,
        };

        // Act
        var response = await UnauthorizedAppClient.Login(request);

        // Assert
        response.AssertSingleBadRequestError(EmailNotConfirmedFailure.Key, EmailNotConfirmedFailure.Message);
    }

    [Fact]
    public async Task GivenUsernameInvalid_ReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with
        {
            Username = RandomData.Username,
        };

        // Act
        var response = await UnauthorizedAppClient.Login(request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Fact]
    public async Task GivenUserIsAuthenticated_ReturnsAnonymousOnlyError()
    {
        // Act
        var response = await AppClient.Login(_request);

        // Assert
        response.AssertAnonymousOnlyError();
    }

    [Fact]
    public async Task GivenPasswordInvalid_ReturnsInvalidFailure()
    {
        // Arrange
        var request = _request with
        {
            Password = RandomData.Password,
        };

        // Act
        var response = await UnauthorizedAppClient.Login(request);

        // Assert
        AssertInvalidFailure(response);
    }

    private static void AssertInvalidFailure(IApiResponse<LoginResponse> response)
    {
        response.AssertSingleBadRequestError(LoginInvalidFailure.Key, LoginInvalidFailure.Message);
    }
}
