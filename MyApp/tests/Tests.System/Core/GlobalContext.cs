using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Tests.System.Core.Settings;
using MyApp.Tests.Utilities.Clients;
using MyApp.Tests.Utilities.Clients.Extensions;
using MyApp.Tests.Utilities.Core;
using Refit;

namespace MyApp.Tests.System.Core;

public static class GlobalContext
{
    public static ServerSettings Settings { get; private set; } = default!;
    public static string AccessToken { get; private set; } = default!;
    public static IServiceProvider Services { get; private set; } = default!;

    public static Task Initialize { get; } = new AsyncLazy(InitializeAsyncInternal).Value;

    private static async Task InitializeAsyncInternal()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.system.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        Settings = ServerSettings.Get(configuration);
        var authSettings = Settings.Auth;

        using var httpClient = new HttpClient
        {
            BaseAddress = new(Settings.BaseUrl),
        };
        var unauthorizedClient = httpClient.ToApplicationClient();

        var loginResponse = await unauthorizedClient.Login(new(authSettings.Username, authSettings.Password, Scopes: []));
        loginResponse.AssertSuccess();
        loginResponse.Content.Should().NotBeNull();
        AccessToken = loginResponse.Content!.AccessToken;
        AccessToken.Should().NotBeNullOrEmpty();


        var services = new ServiceCollection();
        services.AddHttpClient(nameof(TestFixture.AdminAppClient), c =>
        {
            c.BaseAddress = new(Settings.BaseUrl);
            c.SetAuthorizationHeader(AccessToken);
        });
        services.AddHttpClient(nameof(TestFixture.UnauthorizedAppClient), c =>
        {
            c.BaseAddress = new(Settings.BaseUrl);
        });

        Services = services.BuildServiceProvider();
    }
}
