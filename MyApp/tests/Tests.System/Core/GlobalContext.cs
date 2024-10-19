using Microsoft.Extensions.Configuration;
using MyApp.Tests.System.Core.Settings;
using MyApp.Tests.Utilities.Clients;
using MyApp.Tests.Utilities.Core;
using Refit;

namespace MyApp.Tests.System.Core;

public static class GlobalContext
{
    public static ServerSettings Settings { get; private set; } = default!;
    public static string AccessToken { get; private set; } = default!;

    public static Task Initialize { get; } = new AsyncLazy(InitializeAsyncInternal).Value;

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

        await Task.CompletedTask;
    }
}
