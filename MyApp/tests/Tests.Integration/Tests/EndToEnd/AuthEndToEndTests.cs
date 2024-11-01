using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.RefreshToken;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.Register;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ResendConfirmation;
using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Tests.Integration.Tests.EndToEnd;

public class AuthEndToEndTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private const string Email = "olduser@email.com";
    private const string Username = "OldUsername";
    private const string Password = "password1!";
    private IApplicationGraphQLClient _graphqlClient = default!;

    [Fact]
    public async Task HappyPathTest()
    {
        await TestRegister();
        await TestResetPassword(await TestForgotPassword());
        var userConfirmationCode = await TestResendConfirmation();
        await TestConfirmUserRegistration(userConfirmationCode);
        await TestResetPassword(await TestForgotPassword());
        var loginResponse = await TestLogin();
        _graphqlClient = AppFactory.ArrangeGraphQLClientWithToken(loginResponse.AccessToken);
        await TestMe();
        await TestRefreshToken(loginResponse.AccessToken, loginResponse.RefreshToken);
    }

    private async Task TestRegister()
    {
        var request = new RegisterRequest(Email, Username, Password);
        var response = await UnauthorizedAppClient.Register(request);
        response.AssertSuccess();
    }

    private async Task<string> TestResendConfirmation()
    {
        var code = string.Empty;
        var mock = MockBag.Get<IMessageProducer>();
        mock.Setup(m => m.Send(It.IsAny<SendRegisterUserConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<SendRegisterUserConfirmationMessage, CancellationToken>((request, _) => code = request.Code)
            .Returns(Task.CompletedTask);
        var request = new ResendConfirmationRequest(Email);
        var response = await UnauthorizedAppClient.ResendConfirmation(request);
        response.AssertSuccess();
        mock.VerifyAll();
        mock.Reset();
        return code;
    }

    private async Task TestConfirmUserRegistration(string confirmationCode)
    {
        var request = new ConfirmUserRegistrationRequest(confirmationCode);
        var response = await UnauthorizedAppClient.ConfirmUserRegistration(request);
        response.AssertSuccess();
    }

    private async Task<string> TestForgotPassword()
    {
        var code = string.Empty;
        var mock = MockBag.Get<IMessageProducer>();
        mock.Setup(m => m.Send(It.IsAny<SendPasswordResetConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<SendPasswordResetConfirmationMessage, CancellationToken>((request, _) => code = request.Code)
            .Returns(Task.CompletedTask);
        var request = new ForgotPasswordRequest(Email, Username);
        var response = await UnauthorizedAppClient.ForgotPassword(request);
        response.AssertSuccess();
        mock.VerifyAll();
        mock.Reset();
        return code;
    }

    private async Task TestResetPassword(string code)
    {
        var request = new ResetPasswordRequest(code, Password);
        var response = await UnauthorizedAppClient.ResetPassword(request);
        response.AssertSuccess();
    }

    private async Task<LoginResponse> TestLogin()
    {
        var request = new LoginRequest(Username, Password, []);
        var response = await UnauthorizedAppClient.Login(request);
        response.AssertSuccess();
        return response.Content!;
    }

    private async Task TestMe()
    {
        var response = await _graphqlClient.Me.ExecuteAsync();
        response.AssertSuccess();
    }

    private async Task TestRefreshToken(string accessToken, string refreshToken)
    {
        var request = new RefreshTokenRequest(accessToken, refreshToken);
        var response = await UnauthorizedAppClient.RefreshToken(request);
        response.AssertSuccess();
    }
}
