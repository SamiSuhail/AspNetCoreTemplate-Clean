using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Moq;
using MyApp.ApplicationIsolationTests.Core;
using MyApp.Server.Infrastructure.Messaging;
using MyApp.Server.Modules.Commands.Auth.ConfirmEmail;
using MyApp.Server.Modules.Commands.Auth.Login;
using MyApp.Server.Modules.Commands.Auth.PasswordManagement.ForgotPassword;
using MyApp.Server.Modules.Commands.Auth.Register;
using MyApp.Server.Modules.Commands.Auth.Registration;
using MyApp.Server.Modules.Commands.Auth.ResendConfirmation;
using MyApp.Server.Modules.Commands.Auth.ResetPassword;
using MyApp.Server.Modules.Queries.Ping;

namespace MyApp.ApplicationIsolationTests;

public class EndToEndTest : IClassFixture<AppFactory>
{
    // user
    private const string Email = "email@email.com";
    private const string Username = "username";
    private const string Password = "password1!";

    private readonly AppFactory _appFactory;
    private readonly MockBag _mockBag;
    private readonly HttpClient _httpClient;
    private readonly IApplicationClient _client;
    private IApplicationGraphQLClient _graphClient;

    public EndToEndTest(AppFactory appFactory)
    {
        _appFactory = appFactory;
        _mockBag = appFactory.MockBag;
        _httpClient = appFactory.HttpClient;
        _client = appFactory.AppClient;
        _graphClient = appFactory.GraphQLClient;
    }

    [Fact]
    public async Task HappyPathTest()
    {
        await TestUnauthorized();
        await TestPing();
        await TestPingGraphQL();
        await TestRegister();
        var emailConfirmationCode = await TestResendConfirmation();
        await TestConfirmEmail(emailConfirmationCode);
        var passwordResetConfirmationCode = await TestForgotPassword();
        await TestResetPassword(passwordResetConfirmationCode);
        var accessToken = await TestLogin();
        _httpClient.DefaultRequestHeaders.Authorization = new(JwtBearerDefaults.AuthenticationScheme, accessToken);
        _graphClient = _appFactory.CreateGraphQLClient(c => c.DefaultRequestHeaders.Authorization = new(JwtBearerDefaults.AuthenticationScheme, accessToken));
        await TestPingDatabase();
        await TestPingDatabaseGraphQL();
        await TestMeQuery();
    }

    private async Task TestUnauthorized()
    {
        var response = await _graphClient.Me.ExecuteAsync();
        response.Errors.Should().HaveCount(1);
        response.Errors[0].Code.Should().Be("AUTH_NOT_AUTHORIZED");
    }

    private async Task TestPing()
    {
        var response = await _client.Ping();
        response.AssertStatusCode();
        response.Content.Should().NotBeNull();
        response.Content!.Message.Should().Be(nameof(Pong));
    }

    private async Task TestPingGraphQL()
    {
        var response = await _graphClient.Ping.ExecuteAsync();
        response.Errors.Should().BeEmpty();
        response.Data.Should().NotBeNull();
        response.Data!.Ping.Should().NotBeNull();
        response.Data.Ping.Default.Message.Should().Be(nameof(Pong));
    }

    private async Task TestRegister()
    {
        var mock = _mockBag.Get<IMessageProducer>();
        mock.Setup(m => m.Send(It.IsAny<SendEmailConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var request = new RegisterRequest(Email, Username, Password);
        var response = await _client.Register(request);
        response.AssertStatusCode();
        mock.VerifyAll();
        mock.Reset();
    }

    private async Task<string> TestResendConfirmation()
    {
        var code = string.Empty;
        var mock = _mockBag.Get<IMessageProducer>();
        mock.Setup(m => m.Send(It.IsAny<SendEmailConfirmationMessage>(), It.IsAny<CancellationToken>()))
            .Callback<SendEmailConfirmationMessage, CancellationToken>((request, _) => code = request.Code)
            .Returns(Task.CompletedTask);
        var request = new ResendConfirmationRequest(Email);
        var response = await _client.ResendConfirmation(request);
        response.AssertStatusCode();
        mock.VerifyAll();
        mock.Reset();
        return code;
    }

    private async Task TestConfirmEmail(string confirmationCode)
    {
        var request = new ConfirmEmailRequest(confirmationCode);
        var response = await _client.ConfirmEmail(request);
        response.AssertStatusCode();
    }

    private async Task<string> TestForgotPassword()
    {
        var code = string.Empty;
        var mock = _mockBag.Get<IMessageProducer>();
        mock.Setup(m => m.Send(It.IsAny<ForgotPasswordMessage>(), It.IsAny<CancellationToken>()))
            .Callback<ForgotPasswordMessage, CancellationToken>((request, _) => code = request.Code)
            .Returns(Task.CompletedTask);
        var request = new ForgotPasswordRequest(Email, Username);
        var response = await _client.ForgotPassword(request);
        response.AssertStatusCode();
        mock.VerifyAll();
        mock.Reset();
        return code;
    }

    private async Task TestResetPassword(string code)
    {
        var request = new ResetPasswordRequest(code, Password);
        var response = await _client.ResetPassword(request);
        response.AssertStatusCode();
    }

    private async Task<string> TestLogin()
    {
        var request = new LoginRequest(Username, Password);
        var response = await _client.Login(request);
        response.AssertStatusCode();
        response.Content.Should().NotBeNull();
        var accessToken = response.Content!.AccessToken;
        accessToken.Should().NotBeNullOrWhiteSpace();
        return accessToken;
    }

    private async Task TestPingDatabase()
    {
        var response = await _client.PingDatabase();
        response.AssertStatusCode();
        response.Content.Should().NotBeNull();
        response.Content!.Message.Should().Be(nameof(Pong));
    }

    private async Task TestPingDatabaseGraphQL()
    {
        var response = await _graphClient.PingDatabase.ExecuteAsync();
        response.Errors.Should().BeEmpty();
        response.Data.Should().NotBeNull();
        response.Data!.Ping.Should().NotBeNull();
        response.Data.Ping.Database.Message.Should().Be(nameof(Pong));
    }

    private async Task TestMeQuery()
    {
        var response = await _graphClient.Me.ExecuteAsync();
        response.Errors.Should().BeEmpty();
        var profile = response.Data?.Me;
        profile.Should().NotBeNull();
        var user = profile!.User;

        user.Should().NotBeNull();
        using (var _ = new AssertionScope())
        {
            user.Id.Should().BePositive();
            user.Username.Should().Be(Username);
            user.Email.Should().Be(Email);
            user.CreatedAt.ShouldBeToday();
        }
    }
}
