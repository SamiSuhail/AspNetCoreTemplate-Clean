namespace MyApp.Domain.Access.Scope;

public static class CustomScopes
{
    public static string[] All { get; } = [InstanceWrite];

    public const string InstanceWrite = "instance:write";
}