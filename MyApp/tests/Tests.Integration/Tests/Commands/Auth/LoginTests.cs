using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Domain.Auth.User.Failures;

namespace MyApp.Tests.Integration.Tests.Commands.Auth;

public class LoginTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private LoginRequest _request = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _request = new(User.Entity.Username, User.Password, []);
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
        var accessToken = jwtReader.ReadAccessToken(response.Content.AccessToken);
        var refreshToken = jwtReader.ReadRefreshToken(response.Content.RefreshToken);
        accessToken.Should().NotBeNull();
        using (new AssertionScope())
        {
            accessToken!.UserId.Should().Be(User.Entity.Id);
            accessToken.Username.Should().Be(User.Entity.Username);
            accessToken.Email.Should().Be(User.Entity.Email);
        }
        refreshToken.Should().NotBeNull();
        using (new AssertionScope())
        {
            refreshToken!.UserId.Should().Be(User.Entity.Id);
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
        response.AssertInvalidLoginFailure();
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
        response.AssertInvalidLoginFailure();
    }
}
