using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Tests.System.Core.Settings;
using MyApp.Tests.Utilities.Clients;
using MyApp.Tests.Utilities.Clients.Extensions;
using MyApp.Utilities.Collections;
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
            response.AssertSuccess();
            var accessToken = response.Content?.AccessToken;
            accessToken.Should().NotBeNullOrEmpty();
            AccessToken = accessToken!;
            return;
        }

        var problemDetails = response.AssertBadRequest();

        if (problemDetails.Errors.GetValueOrDefault(UserRegistrationNotConfirmedFailure.Key)?.Any(message => message == UserRegistrationNotConfirmedFailure.Message) == true)
        {
            var resendConfirmationResponse = await client.ResendConfirmation(new(authSettings.Email));
            resendConfirmationResponse.AssertSuccess();
        }
        else
        {
            response.AssertInvalidLoginFailure();

            var registerResponse = await client.Register(new(authSettings.Email, authSettings.Username, authSettings.Password));
            registerResponse.AssertSuccess();
        }

        var confirmationResponse = await client.ConfirmUserRegistration(new("000000"));
        confirmationResponse.AssertSuccess();

        response = await client.Login(new(authSettings.Username, authSettings.Password));
        response.AssertSuccess();
        var token = response.Content?.AccessToken;
        token.Should().NotBeNullOrEmpty();
        AccessToken = token!;
    }
}
