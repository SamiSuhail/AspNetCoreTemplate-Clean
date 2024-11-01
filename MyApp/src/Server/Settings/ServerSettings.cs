using MyApp.Utilities.Settings;

namespace MyApp.Server.Settings;

public class ServerSettings : BaseSettings<ServerSettings>
{
    private string[]? _urls;
    const char _separator = ';';

    public required string UrlsJoined { get; set; }

    public string[] Urls => _urls ??= UrlsJoined.Split(_separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
