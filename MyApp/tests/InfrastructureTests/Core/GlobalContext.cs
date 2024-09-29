using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.InfrastructureTests.Core;

public static class GlobalContext
{
    public static IConfigurationRoot Configuration { get; } = CreateConfiguration();

    private static IConfigurationRoot CreateConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder.AddInfrastructureAppsettings()
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
