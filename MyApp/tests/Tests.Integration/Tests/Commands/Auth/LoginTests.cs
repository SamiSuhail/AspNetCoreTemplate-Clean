using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Interfaces.Commands.Auth.Login;
using MyApp.Domain.Auth.User.Failures;

namespace MyApp.Tests.Integration.Tests.Commands.Auth;

public class LoginTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private LoginRequest _request = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _request = new(User.Entity.Username, User.Password);
    }

    [Fact]
    public async Task GivenHappyPath_ReturnsValidTokens()
    {
        // Act
        var response = await UnauthorizedAppClient.Login(_request);

        // Assert
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        response.Content!.AccessToken.Should().NotBeNullOrEmpty();
        response.Content!.RefreshToken.Should().NotBeNullOrEmpty();

        var jwtReader = ScopedServices.GetRequiredService<IJwtReader>();
        var user = jwtReader.ReadAccessToken(response.Content.AccessToken);
        var refreshToken = jwtReader.ReadRefreshToken(response.Content.RefreshToken);
        using (new AssertionScope())
        {
            user.Id.Should().Be(User.Entity.Id);
            user.Username.Should().Be(User.Entity.Username);
            user.Email.Should().Be(User.Entity.Email);
        }
        refreshToken.Should().NotBeNull();
        using (new AssertionScope())
        {
            refreshToken!.UserId.Should().Be(User.Entity.Id);
            refreshToken.Username.Should().Be(User.Entity.Username);
            refreshToken.Email.Should().Be(User.Entity.Email);
            refreshToken.Version.Should().Be(User.Entity.RefreshTokenVersion);
        }
    }

    [Fact]
    public async Task GivenEmailNotConfirmed_ReturnsEmailNotConfirmedFailure()
    {
        // Arrange
        var (user, password) = await ArrangeDbContext.ArrangeRandomUnconfirmedUserWithPassword();
        var request = _request with
        {
            Username = user.Username,
            Password = password,
        };

        // Act
        var response = await UnauthorizedAppClient.Login(request);

        // Assert
        response.AssertSingleBadRequestError(UserRegistrationNotConfirmedFailure.Key, UserRegistrationNotConfirmedFailure.Message);
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
