namespace MyApp.Tests.System.Tests.Auth;

public class RegisterTests(TestFixture fixture) : BaseTest(fixture)
{
    private string _instanceName = default!;
    private UserCredentials _userCredentials;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _instanceName = await AdminAppClient.ArrangeRandomInstance();
        _userCredentials = (RandomData.Username, RandomData.Password, GlobalContext.EmailSettings.Users.Default.EmailAddress);
    }

    [Fact]
    public async Task GivenHappyPath_ThenConfirmationCodeReceived()
    {
        // Arrange
        var (username, password, email) = _userCredentials;

        // Act
        var response = await UnauthorizedAppClient.Register(new(email, username, password), _instanceName);

        // Assert
        response.AssertSuccess();

        var code = await GlobalContext.EmailProvider
            .GetUserConfirmationCode(username);
        code.AssertValidConfirmationCode();
    }
}
