using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MyApp.Application.Interfaces.Commands.Auth.Login;
using MyApp.Tests.System.Core.Settings;
using MyApp.Tests.Utilities.Clients;
using MyApp.Tests.Utilities.Clients.Extensions;
using Refit;

namespace MyApp.Tests.System.Core;

public static class GlobalContext
{
    public static ServerSettings Settings { get; private set; } = default!;
    public static string AccessToken { get; private set; } = default!;

    public static Task Initialize { get; } = new Lazy<Task>(() => Task.Run(InitializeAsyncInternal)).Value;

    private static async Task InitializeAsyncInternal()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.system.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        Settings = ServerSettings.Get(configuration);
        var authSettings = Settings.Auth;

        var httpClient = new HttpClient
        {
            BaseAddress = new(Settings.BaseUrl),
        };
        var client = RestService.For<IApplicationClient>(httpClient);

        var response = await client.Login(new(authSettings.Username, authSettings.Password));

        if (response.IsSuccessStatusCode)
        {
            AssertAndSetAccessToken(response);
            return;
        }

        response.AssertInvalidLoginFailure();

        var registerResponse = await client.Register(new(authSettings.Email, authSettings.Username, authSettings.Password));
        registerResponse.AssertSuccess();

        var confirmationResponse = await client.ConfirmUserRegistration(new("000000"));
        confirmationResponse.AssertSuccess();

        response = await client.Login(new(authSettings.Username, authSettings.Password));
        AssertAndSetAccessToken(response);
    }

    private static void AssertAndSetAccessToken(IApiResponse<LoginResponse> response)
    {
        response.AssertSuccess();
        var token = response.Content?.AccessToken;
        token.Should().NotBeNullOrEmpty();
        AccessToken = token!;
    }
}
