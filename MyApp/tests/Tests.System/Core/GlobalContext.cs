using MyApp.Tests.System.Providers.Email;

namespace MyApp.Tests.System.Core;

public static class GlobalContext
{
    public static ServerSettings ServerSettings { get; private set; } = default!;
    public static EmailSettings EmailSettings { get; private set; } = default!;
    public static string AccessToken { get; private set; } = default!;
    public static IServiceProvider Services { get; private set; } = default!;
    public static EmailProvider EmailProvider => Services.GetRequiredService<EmailProvider>();

    public static Task Initialize { get; } = new AsyncLazy(InitializeAsyncInternal).Value;

    private static async Task InitializeAsyncInternal()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("testsettings.system.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        ServerSettings = ServerSettings.Get(configuration);
        EmailSettings = EmailSettings.Get(configuration);
        var authSettings = ServerSettings.AdminAuth;

        using var httpClient = new HttpClient
        {
            BaseAddress = new(ServerSettings.BaseUrl),
        };
        var unauthorizedClient = httpClient.ToApplicationClient();

        var loginResponse = await unauthorizedClient.Login(new(authSettings.Username, authSettings.Password, Scopes: []));
        loginResponse.AssertSuccess();
        loginResponse.Content.Should().NotBeNull();
        AccessToken = loginResponse.Content!.AccessToken;
        AccessToken.Should().NotBeNullOrEmpty();


        var services = new ServiceCollection();
        services.AddHttpClient(nameof(BaseTest.AdminAppClient), c =>
        {
            c.BaseAddress = new(ServerSettings.BaseUrl);
            c.SetAuthorizationHeader(AccessToken);
        });
        services.AddHttpClient(nameof(BaseTest.UnauthorizedAppClient), c =>
        {
            c.BaseAddress = new(ServerSettings.BaseUrl);
        });

        services.AddSingleton(EmailSettings);
        services.AddSingleton<EmailProvider>();

        Services = services.BuildServiceProvider();
    }
}
