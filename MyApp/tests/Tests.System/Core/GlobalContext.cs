using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MyApp.Application.Interfaces.Commands.Auth.Login;
using MyApp.Application.Interfaces.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Application.Interfaces.Commands.Auth.Registration.Register;
using MyApp.Tests.System.Core.Settings;
using MyApp.Tests.Utilities.Clients;
using MyApp.Tests.Utilities.Clients.Extensions;
using Refit;

namespace MyApp.Tests.System.Core;

public class GlobalContext
{
    public GlobalContext()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.system.json", optional: false)
            .AddEnvironmentVariables()
            .Build();
    }

    public IConfiguration Configuration { get; private set; }
    public IApplicationClient UnauthorizedAppClient { get; private set; } = default!;
    public IApplicationClient AppClient { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var settings = ServerSettings.Get(Configuration);
        var authSettings = settings.Auth;

        var httpClient = new HttpClient
        {
            BaseAddress = new(settings.BaseUrl),
        };
        UnauthorizedAppClient = RestService.For<IApplicationClient>(httpClient);

        var authorizedHttpClient = new HttpClient
        {
            BaseAddress = new(settings.BaseUrl),
        };

        var loginRequest = new LoginRequest(authSettings.Username, authSettings.Password);
        var loginResponse = await UnauthorizedAppClient.Login(loginRequest);

        if (!loginResponse.IsSuccessStatusCode)
        {
            loginResponse.AssertInvalidLoginFailure();

            var registerRequest = new RegisterRequest(authSettings.Email, authSettings.Username, authSettings.Password);
            var registerResponse = await UnauthorizedAppClient.Register(registerRequest);
            registerResponse.AssertSuccess();

            var confirmationRequest = new ConfirmUserRegistrationRequest("000000");
            var confirmationResponse = await UnauthorizedAppClient.ConfirmUserRegistration(confirmationRequest);
            confirmationResponse.AssertSuccess();

            loginResponse = await UnauthorizedAppClient.Login(loginRequest);
            loginResponse.AssertSuccess();
        }

        var accessToken = loginResponse.Content?.AccessToken;
        accessToken.Should().NotBeNullOrEmpty();
        authorizedHttpClient.SetAuthorizationHeader(accessToken!);
        AppClient = RestService.For<IApplicationClient>(authorizedHttpClient);
    }
}
