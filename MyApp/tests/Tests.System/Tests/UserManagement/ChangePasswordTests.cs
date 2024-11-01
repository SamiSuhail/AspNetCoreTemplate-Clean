namespace MyApp.Tests.System.Tests.UserManagement;

public class ChangePasswordTests(TestFixture fixture) : AuthenticatedBaseTest(fixture)
{
    [Fact]
    public async Task GivenHappyPath_ThenConfirmationCodeReceived()
    {
        // Act
        var response = await AppClient.ChangePassword();

        // Assert
        response.AssertSuccess();
        var code = await GlobalContext.EmailProvider
            .GetPasswordResetConfirmationCode(UserCredentials.Username);
        code.AssertValidConfirmationCode();
    }
}
