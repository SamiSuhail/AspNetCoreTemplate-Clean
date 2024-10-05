using MyApp.Application.Commands.UserManagement.EmailUpdate.ChangeEmail;
using MyApp.Application.Infrastructure.Abstractions;

namespace MyApp.Tests.Integration.Tests.EndToEnd;

public class UserManagementEndToEndTests(AppFactory appFactory) : BaseTest(appFactory)
{
    [Fact]
    public async Task HappyPathTest()
    {
        await TestSignOutOnAllDevices();
        await TestRefreshTokenFailure(User.RefreshToken);
        var (oldEmailCode, newEmailCode) = await TestChangeEmail();
        await TestConfirmEmailChange(oldEmailCode, newEmailCode);
    }

    private async Task TestSignOutOnAllDevices()
    {
        var response = await AppClient.SignOutOnAllDevices();
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
}
