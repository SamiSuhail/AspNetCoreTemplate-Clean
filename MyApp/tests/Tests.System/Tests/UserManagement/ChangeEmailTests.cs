namespace MyApp.Tests.System.Tests.UserManagement;

public class ChangeEmailTests(TestFixture fixture) : AuthenticatedBaseTest(fixture)
{
    [Fact]
    public async Task GivenHappyPath_ThenConfirmationCodeReceived()
    {
        // Arrange
        var newEmailAddress = GlobalContext.EmailSettings.Users.Other.EmailAddress;

        // Act
        var response = await AppClient.ChangeEmail(new(newEmailAddress));

        // Assert
        response.AssertSuccess();
        var oldEmailCode = await GlobalContext.EmailProvider
            .GetChangeEmailConfirmationCode(UserCredentials.Username, users => users.Default);
        oldEmailCode.AssertValidConfirmationCode();
        var newEmailCode = await GlobalContext.EmailProvider
            .GetChangeEmailConfirmationCode(UserCredentials.Username, users => users.Other);
        oldEmailCode.AssertValidConfirmationCode();
    }
}
