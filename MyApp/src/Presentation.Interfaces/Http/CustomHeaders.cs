namespace MyApp.Presentation.Interfaces.Http;

public static class CustomHeaders
{
    public const string Prefix = "MyApp-";

    public const string InstanceName = $"{Prefix}{nameof(InstanceName)}";
}
