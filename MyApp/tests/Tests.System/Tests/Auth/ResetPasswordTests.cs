
namespace MyApp.Tests.System.Tests.Auth;

public class ForgotPasswordTests(TestFixture fixture) : AuthenticatedBaseTest(fixture)
{
    [Fact]
    public async Task GivenHappyPath_ThenConfirmationCodeReceived()
    {
        // Act
        var response = await UnauthorizedAppClient.ForgotPassword(new(UserCredentials.Email, UserCredentials.Username));

        // Assert
        response.AssertSuccess();
        var code = await GlobalContext.EmailProvider
            .GetPasswordResetConfirmationCode(UserCredentials.Username);
        code.AssertValidConfirmationCode();
    }
}
