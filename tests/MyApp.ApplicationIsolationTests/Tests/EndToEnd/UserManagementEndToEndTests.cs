using MyApp.Server.Application.Commands.Auth.Login;
using MyApp.Server.Application.Commands.UserManagement.EmailUpdate.ChangeEmail;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.ApplicationIsolationTests.Tests.EndToEnd;

public class UserManagementEndToEndTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private UserEntity _user = default!;
    private string _password = default!;
    private IApplicationClient _client = default!;

    [Fact]
    public async Task HappyPathTest()
    {
        (_user, _password) = await ArrangeDbContext.ArrangeRandomConfirmedUserWithPassword();
        var loginResponse = await TestLogin();
        _client = AppFactory.CreateClientWithToken(loginResponse.AccessToken);
        await TestSignOutOnAllDevices();
        await TestRefreshTokenFailure(loginResponse.RefreshToken);
        var (oldEmailCode, newEmailCode) = await TestChangeEmail();
        await TestConfirmEmailChange(oldEmailCode, newEmailCode);
    }

    private async Task<LoginResponse> TestLogin()
    {
        var response = await UnauthorizedAppClient.Login(new(_user.Username, _password));
        response.AssertSuccess();
        return response.Content!;
    }

    private async Task TestSignOutOnAllDevices()
    {
        var response = await _client.SignOutOnAllDevices();
        response.AssertSuccess();
    }

    private async Task TestRefreshTokenFailure(string refreshToken)
    {
        var response = await UnauthorizedAppClient.RefreshToken(new(refreshToken));
        response.AssertBadRequest();
    }

    private async Task<(string OldEmailCode, string NewEmailCode)> TestChangeEmail()
    {
        ChangeEmailMessage message = null!;
        MockBag.Get<IMessageProducer>()
            .Setup(m => m.Send(It.IsAny<ChangeEmailMessage>(), It.IsAny<CancellationToken>()))
            .Callback<ChangeEmailMessage, CancellationToken>((m, _) => message = m)
            .Returns(Task.CompletedTask);

        var response = await _client.ChangeEmail(new(RandomData.Email));

        response.AssertSuccess();
        message.Should().NotBeNull();
        MockBag.VerifyAll();
        MockBag.Reset();
        return (message.OldEmailCode, message.NewEmailCode);
    }

    private async Task TestConfirmEmailChange(string oldEmailCode, string newEmailCode)
    {
        var response = await _client.ConfirmEmailChange(new(oldEmailCode, newEmailCode));
        response.AssertSuccess();
    }
}
