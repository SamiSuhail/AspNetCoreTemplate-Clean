namespace MyApp.Server.Shared;
public class BaseBackgroundJob<TActual>
{
    public static string Name { get; } = typeof(TActual).Name;
}
