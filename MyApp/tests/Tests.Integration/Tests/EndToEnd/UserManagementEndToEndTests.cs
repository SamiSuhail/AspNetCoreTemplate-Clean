using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Tests.Integration.Tests.EndToEnd;

public class UserManagementEndToEndTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task HappyPathTest()
    {
        await TestSignOutOnAllDevices();
        await TestRefreshTokenFailure(User.AccessToken, User.RefreshToken);
        var (oldEmailCode, newEmailCode) = await TestChangeEmail();
        await TestConfirmEmailChange(oldEmailCode, newEmailCode);
        await TestConfirmPasswordChange(await TestChangePassword());
    }

    private async Task TestSignOutOnAllDevices()
    {
        var response = await AppClient.SignOutOnAllDevices();
        response.AssertSuccess();
    }

    private async Task TestRefreshTokenFailure(string accessToken, string refreshToken)
    {
        var response = await UnauthorizedAppClient.RefreshToken(new(accessToken, refreshToken));
        response.AssertBadRequest();
    }

    private async Task<(string OldEmailCode, string NewEmailCode)> TestChangeEmail()
    {
        SendChangeEmailConfirmationMessage message = null!;
        MockBag.Get<IMessageProducer>()
            .Setup(m => m.Send(It.IsAny<SendChangeEmailConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<SendChangeEmailConfirmationMessage, CancellationToken>((m, _) => message = m)
            .Returns(Task.CompletedTask);

        var response = await AppClient.ChangeEmail(new(RandomData.Email));

        response.AssertSuccess();
        message.Should().NotBeNull();
        MockBag.VerifyAll();
        MockBag.Reset();
        return (message.OldEmailCode, message.NewEmailCode);
    }

    private async Task TestConfirmEmailChange(string oldEmailCode, string newEmailCode)
    {
        var response = await AppClient.ConfirmEmailChange(new(oldEmailCode, newEmailCode));
        response.AssertSuccess();
    }

    private async Task<string> TestChangePassword()
    {
        var code = string.Empty;
        var mock = MockBag.Get<IMessageProducer>();
        mock.Setup(m => m.Send(It.IsAny<SendPasswordResetConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<SendPasswordResetConfirmationMessage, CancellationToken>((request, _) => code = request.Code)
            .Returns(Task.CompletedTask);
        var response = await AppClient.ChangePassword();
        response.AssertSuccess();
        mock.VerifyAll();
        mock.Reset();
        return code;
    }

    private async Task TestConfirmPasswordChange(string code)
    {
        var request = new ConfirmPasswordChangeRequest(code, User.Password);
        var response = await AppClient.ConfirmPasswordChange(request);
        response.AssertSuccess();
    }
}
