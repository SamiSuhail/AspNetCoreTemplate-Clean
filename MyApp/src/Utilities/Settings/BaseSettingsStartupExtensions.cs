using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Utilities.Settings;

public static class BaseSettingsStartupExtensions
{
    public static TSettings AddCustomSettings<TSettings>(
        this IServiceCollection services,
        IConfiguration configuration
        ) where TSettings : BaseSettings<TSettings>
    {
        var settings = BaseSettings<TSettings>.Get(configuration);
        services.AddSingleton(settings);

        return settings;
    }
}
