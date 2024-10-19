namespace MyApp.Tests.Integration.Tests.Commands.UserManagement;

public class SignOutOnAllDevicesTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task GivenHappyPath_ThenIncrementsRefreshTokenVersion()
    {
        // Act
        var response = await AppClient.SignOutOnAllDevices();

        // Assert
        response.AssertSuccess();
        var user = await AssertDbContext.GetUser(User.Entity.Id);
        user.RefreshTokenVersion.Should().Be(User.Entity.RefreshTokenVersion + 1);
    }

    [Fact]
    public async Task GivenUserIdNotFound_ReturnsUserNotFoundFailure()
    {
        // Arrange
        var client = AppFactory.ArrangeClientWithCredentials(userId: int.MaxValue, User.Entity.Username, User.Entity.Email);

        // Act
        var response = await client.SignOutOnAllDevices();

        // Assert
        response.AssertUserNotFoundFailure();
    }
}
