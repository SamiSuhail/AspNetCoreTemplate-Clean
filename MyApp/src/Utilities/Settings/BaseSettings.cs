using Microsoft.Extensions.Configuration;

namespace MyApp.Utilities.Settings;

public abstract class BaseSettings<TActual> where TActual : class
{
    private static readonly string _typeName;

    static BaseSettings()
    {
        _typeName = typeof(TActual).Name;
        SectionName = _typeName.Replace("Settings", string.Empty);
    }
    public static string SectionName { get; }

    public static TActual Get(IConfiguration configuration)
        => configuration.GetSection(SectionName).Get<TActual>()
        ?? throw new Exception($"Settings for {_typeName} not found in section {SectionName}.");
}
