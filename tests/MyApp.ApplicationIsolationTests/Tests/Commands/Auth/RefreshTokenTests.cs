using System.Security.Cryptography;
using MyApp.ApplicationIsolationTests.Clients;
using MyApp.Server.Application.Commands.Auth.RefreshToken;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure;
using MyApp.Server.Infrastructure.Auth;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.Auth;

public class RefreshTokenTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private IApplicationClient _client = default!;
    private RefreshTokenRequest _request = default!;

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenHappyPath_ReturnsTokens(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);
        ArrangeRequest(User.Id, User.RefreshTokenVersion);
        await Task.Delay(TimeSpan.FromSeconds(1)); // waiting to guarantee the response will have different expiration

        // Act
        var response = await _client.RefreshToken(_request);

        // Assert
        var jwtReader = ScopedServices.GetRequiredService<IJwtReader>();
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        using (new AssertionScope())
        {
            response.Content!.AccessToken.Should().NotBeNullOrEmpty();
            response.Content.RefreshToken.Should().NotBeNullOrEmpty();
        }
        var user = jwtReader.ReadAccessToken(response.Content.AccessToken);
        user.Should().NotBeNull();
        using (new AssertionScope())
        {
            user.Id.Should().Be(User.Id);
            user.Username.Should().Be(User.Username);
            user.Email.Should().Be(User.Email);
        }
        var refreshToken = jwtReader.ReadRefreshToken(response.Content.RefreshToken);
        refreshToken.Should().NotBeNull();
        using (new AssertionScope())
        {
            response.Content.RefreshToken.Should().NotBe(_request.RefreshToken);
            refreshToken!.UserId.Should().Be(User.Id);
            refreshToken.Username.Should().Be(User.Username);
            refreshToken.Email.Should().Be(User.Email);
            refreshToken.Version.Should().Be(User.RefreshTokenVersion);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenUserNotFound_ReturnsInvalidFailure(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);
        ArrangeRequest(userId: int.MaxValue, User.RefreshTokenVersion);

        // Act
        var response = await _client.RefreshToken(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenInvalidTokenVersion_ReturnsInvalidFailure(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);
        ArrangeRequest(User.Id, refreshTokenVersion: -1);

        // Act
        var response = await _client.RefreshToken(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenExpiredToken_ReturnsInvalidFailure(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);
        var expiredToken = ScopedServices.ArrangeExpiredRefreshToken(User.RefreshTokenVersion);
        _request = new(expiredToken);

        // Act
        var response = await _client.RefreshToken(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenInvalidToken_ReturnsInvalidFailure(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);
        var jwtGenerator = ScopedServices.ArrangeJwtGeneratorWithInvalidPrivateKey();
        var token = jwtGenerator.CreateRefreshToken(User.Id, User.Username, User.Email, User.RefreshTokenVersion);
        _request = new(token);

        // Act
        var response = await _client.RefreshToken(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    private static void AssertInvalidFailure(IApiResponse<RefreshTokenResponse> response)
    {
        response.AssertSingleBadRequestError(UserSessionCouldNotBeRefreshedFailure.Key, UserSessionCouldNotBeRefreshedFailure.Message);
    }

    private void ArrangeClient(bool userIsAuthenticated)
    {
        _client = userIsAuthenticated
            ? AppClient
            : UnauthorizedAppClient;
    }

    private void ArrangeRequest(int userId, int refreshTokenVersion)
    {
        var requestRefreshToken = ScopedServices.GetRequiredService<IJwtGenerator>()
            .CreateRefreshToken(userId, User.Username, User.Email, refreshTokenVersion);
        _request = new(requestRefreshToken);
    }
}
