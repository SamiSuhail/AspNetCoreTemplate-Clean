using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Auth;

namespace MyApp.ApplicationIsolationTests.Tests.Commands.UserManagement;

public class SignOutOnAllDevicesTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ThenIncrementsRefreshTokenVersion()
    {
        // Act
        var response = await AppClient.SignOutOnAllDevices();

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Id);
        user.Should().NotBeNull();
        user!.RefreshTokenVersion.Should().Be(User.RefreshTokenVersion + 1);
    }

    [Fact]
    public async Task GivenUserUnauthorized_ReturnsUnauthorizedError()
    {
        // Act
        var response = await UnauthorizedAppClient.SignOutOnAllDevices();

        // Assert
        response.AssertUnauthorizedError();
    }

    [Fact]
    public async Task GivenUserIdNotFound_ReturnsUserNotFoundFailure()
    {
        var accessToken = ScopedServices.GetRequiredService<IJwtGenerator>()
            .CreateAccessToken(userId: int.MaxValue, User.Username, User.Email);
        var client = AppFactory.CreateClientWithToken(accessToken);

        // Act
        var response = await client.SignOutOnAllDevices();

        // Assert
        response.AssertSingleBadRequestError(UserIdNotFoundFailure.Key, UserIdNotFoundFailure.Message);
    }
}
