namespace MyApp.Server.Shared;

public static class TestsHelper
{
    public static bool RandomizeBackgroundJobNames { get; set; } = false;
    public static string GetName(string backgroundJobName)
        => RandomizeBackgroundJobNames
            ? $"{backgroundJobName}-{Guid.NewGuid()}"
            : backgroundJobName;
}
