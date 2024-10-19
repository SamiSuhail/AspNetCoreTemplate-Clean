namespace MyApp.Tests.Integration.Utilities;

public static class RandomData
{
    public static char[] AlphanumericCharacters { get; } = Enumerable.Range('0', 10)
        .Concat(Enumerable.Range('a', 26))
        .Concat(Enumerable.Range('A', 26))
        .Select(x => (char) x)
        .ToArray();

    public static string Alphanumeric(int length = 10)
    {
        var random = new Random();

        var chars = Enumerable.Range (0, length)
            .Select(_ => AlphanumericCharacters[random.Next(AlphanumericCharacters.Length)])
            .ToArray();

        return new(chars);
    }

    public static string InstanceName => Alphanumeric();
    public static string Username => Alphanumeric();
    public static string Email => Alphanumeric() + "@email.com";
    public static string Password => Alphanumeric();
    public static string ScopeName => Alphanumeric();
}
