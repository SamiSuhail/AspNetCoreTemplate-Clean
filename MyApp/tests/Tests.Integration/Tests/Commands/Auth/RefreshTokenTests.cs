using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Domain.Access.Scope;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.RefreshToken;

namespace MyApp.Tests.Integration.Tests.Commands.Auth;

public class RefreshTokenTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private IApplicationClient _client = default!;
    private RefreshTokenRequest _request = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _request = NewRequest(User.Entity.Id, User.Entity.RefreshTokenVersion);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenHappyPath_ReturnsTokens(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);
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
        var accessToken = jwtReader.ReadAccessToken(response.Content.AccessToken);
        accessToken.Should().NotBeNull();
        using (new AssertionScope())
        {
            accessToken!.UserId.Should().Be(User.Entity.Id);
            accessToken.Username.Should().Be(User.Entity.Username);
            accessToken.Email.Should().Be(User.Entity.Email);
        }
        var refreshToken = jwtReader.ReadRefreshToken(response.Content.RefreshToken);
        refreshToken.Should().NotBeNull();
        using (new AssertionScope())
        {
            response.Content.RefreshToken.Should().NotBe(_request.RefreshToken);
            refreshToken!.UserId.Should().Be(User.Entity.Id);
            refreshToken.Version.Should().Be(User.Entity.RefreshTokenVersion);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenUserNotFound_ReturnsInvalidFailure(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);
        _request = NewRequest(userId: int.MaxValue, User.Entity.RefreshTokenVersion);

        // Act
        var response = await _client.RefreshToken(_request);

        // Assert
        response.AssertUserNotFoundFailure();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenInvalidTokenVersion_ReturnsInvalidFailure(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);
        _request = NewRequest(User.Entity.Id, refreshTokenVersion: -1);

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
        var expiredToken = ScopedServices.ArrangeExpiredRefreshToken(User);
        _request = _request with
        {
            RefreshToken = expiredToken,
        };

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
        var token = jwtGenerator.CreateRefreshToken(User.Entity.Id, User.Entity.RefreshTokenVersion);
        _request = _request with
        {
            RefreshToken = token,
        };

        // Act
        var response = await _client.RefreshToken(_request);

        // Assert
        AssertInvalidFailure(response);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenTokensAreFromDifferentUsers_ReturnsInvalidFailure(bool userIsAuthenticated)
    {
        // Arrange
        ArrangeClient(userIsAuthenticated);

        var otherUser = await ArrangeDbContext.ArrangeRandomConfirmedUser();

        var jwtGenerator = ScopedServices.ArrangeJwtGeneratorWithInvalidPrivateKey();
        var token = jwtGenerator.CreateRefreshToken(otherUser.Id, otherUser.RefreshTokenVersion);
        _request = _request with
        {
            RefreshToken = token,
        };

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

    private RefreshTokenRequest NewRequest(int userId, int refreshTokenVersion)
    {
        var jwtGenerator = ScopedServices.GetRequiredService<IJwtGenerator>();
        var requestRefreshToken = jwtGenerator.CreateRefreshToken(userId, refreshTokenVersion);
        var expiredAccessToken = jwtGenerator.CreateAccessToken(userId, User.Entity.Username, User.Entity.Email, ScopeCollection.Empty);
        return new(expiredAccessToken, requestRefreshToken);
    }
}
