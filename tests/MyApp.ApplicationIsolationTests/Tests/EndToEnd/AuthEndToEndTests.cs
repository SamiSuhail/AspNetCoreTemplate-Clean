using MyApp.Server.Application.Commands.Auth.Login;
using MyApp.Server.Application.Commands.Auth.PasswordManagement.ForgotPassword;
using MyApp.Server.Application.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Server.Application.Commands.Auth.Registration;
using MyApp.Server.Application.Commands.Auth.Registration.ConfirmEmail;
using MyApp.Server.Application.Commands.Auth.Registration.Register;
using MyApp.Server.Application.Commands.Auth.Registration.ResendConfirmation;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.ApplicationIsolationTests.Tests.EndToEnd;

public class AuthEndToEndTests(AppFactory appFactory) : BaseTest(appFactory)
{
    // user
    private const string Email = "olduser@email.com";
    private const string Username = "OldUsername";
    private const string Password = "password1!";

    [Fact]
    public async Task HappyPathTest()
    {
        await TestRegister();
        var emailConfirmationCode = await TestResendConfirmation();
        await TestConfirmEmail(emailConfirmationCode);
        var passwordResetConfirmationCode = await TestForgotPassword();
        await TestResetPassword(passwordResetConfirmationCode);
        await TestLogin();
    }

    private static async Task TestRegister()
    {
        var request = new RegisterRequest(Email, Username, Password);
        var response = await UnauthorizedAppClient.Register(request);
        response.AssertSuccess();
    }

    private static async Task<string> TestResendConfirmation()
    {
        var code = string.Empty;
        var mock = MockBag.Get<IMessageProducer>();
        mock.Setup(m => m.Send(It.IsAny<SendEmailConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<SendEmailConfirmationMessage, CancellationToken>((request, _) => code = request.Code)
            .Returns(Task.CompletedTask);
        var request = new ResendConfirmationRequest(Email);
        var response = await UnauthorizedAppClient.ResendConfirmation(request);
        response.AssertSuccess();
        mock.VerifyAll();
        mock.Reset();
        return code;
    }

    private static async Task TestConfirmEmail(string confirmationCode)
    {
        var request = new ConfirmEmailRequest(confirmationCode);
        var response = await UnauthorizedAppClient.ConfirmEmail(request);
        response.AssertSuccess();
    }

    private static async Task<string> TestForgotPassword()
    {
        var code = string.Empty;
        var mock = MockBag.Get<IMessageProducer>();
        mock.Setup(m => m.Send(It.IsAny<ForgotPasswordMessage>(), It.IsAny<CancellationToken>()))
            .Callback<ForgotPasswordMessage, CancellationToken>((request, _) => code = request.Code)
            .Returns(Task.CompletedTask);
        var request = new ForgotPasswordRequest(Email, Username);
        var response = await UnauthorizedAppClient.ForgotPassword(request);
        response.AssertSuccess();
        mock.VerifyAll();
        mock.Reset();
        return code;
    }

    private static async Task TestResetPassword(string code)
    {
        var request = new ResetPasswordRequest(code, Password);
        var response = await UnauthorizedAppClient.ResetPassword(request);
        response.AssertSuccess();
    }

    private async Task TestLogin()
    {
        var request = new LoginRequest(Username, Password);
        var response = await UnauthorizedAppClient.Login(request);
        response.AssertSuccess();
        response.Content.Should().NotBeNull();
        var accessToken = response.Content!.AccessToken;
        accessToken.Should().NotBeNullOrWhiteSpace();
        var userContext = ScopedServices.GetRequiredService<IJwtReader>()
            .ReadAccessToken(accessToken);
        using var _ = new AssertionScope();
        userContext.Username.Should().Be(Username);
        userContext.Email.Should().Be(Email);
    }
}
