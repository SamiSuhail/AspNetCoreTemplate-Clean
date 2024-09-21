using MyApp.Infrastructure.Utilities;

namespace MyApp.Infrastructure.Startup;

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
