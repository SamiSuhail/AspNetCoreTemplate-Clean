using MyApp.ApplicationIsolationTests.Clients;
using MyApp.Server.Application.Commands.Auth.Login;
using MyApp.Server.Application.Commands.Auth.RefreshToken;

namespace MyApp.ApplicationIsolationTests.Tests.EndToEnd;

public class UserManagementEndToEndTests(AppFactory appFactory) : BaseTest(appFactory)
{
    private IApplicationClient _client = default!;

    [Fact]
    public async Task HappyPathTest()
    {
        var loginResponse = await TestLogin();
        _client = AppFactory.CreateClientWithToken(loginResponse.AccessToken);
        await TestSignOutOnAllDevices();
        await TestRefreshTokenFailure(loginResponse.RefreshToken);
    }

    private static async Task<LoginResponse> TestLogin()
    {
        var request = new LoginRequest(TestUser.Username, TestUser.Password);
        var response = await UnauthorizedAppClient.Login(request);
        response.AssertSuccess();
        return response.Content!;
    }

    private async Task TestSignOutOnAllDevices()
    {
        var response = await _client.SignOutOnAllDevices();
        response.AssertSuccess();
    }

    private static async Task TestRefreshTokenFailure(string refreshToken)
    {
        var request = new RefreshTokenRequest(refreshToken);
        var response = await UnauthorizedAppClient.RefreshToken(request);
        response.AssertBadRequest();
    }
}
