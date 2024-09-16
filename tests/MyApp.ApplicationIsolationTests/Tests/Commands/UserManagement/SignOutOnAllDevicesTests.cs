using MyApp.Server.Domain.Auth.User.Failures;

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
        user.RefreshTokenVersion.Should().Be(User.RefreshTokenVersion + 1);
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
        // Arrange
        var client = AppFactory.CreateClientWithCredentials(userId: int.MaxValue, User.Username, User.Email);

        // Act
        var response = await client.SignOutOnAllDevices();

        // Assert
        response.AssertSingleBadRequestError(UserIdNotFoundFailure.Key, UserIdNotFoundFailure.Message);
    }
}
