namespace MyApp.Application.Utilities;
public class BaseBackgroundJob<TActual>
{
    public static string Name { get; } = typeof(TActual).Name;
}
