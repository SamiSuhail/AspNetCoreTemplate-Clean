namespace MyApp.Tests.Utilities.Arrange;

public static class RandomData
{
    private static readonly Random _random = new(1337);
    private static char[] AlphanumericCharacters { get; } = Enumerable.Range('0', 10)
        .Concat(Enumerable.Range('a', 26))
        .Concat(Enumerable.Range('A', 26))
        .Select(x => (char)x)
        .ToArray();

    private static string Alphanumeric(int length = 10)
    {
        Span<char> chars = stackalloc char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = AlphanumericCharacters[_random.Next(AlphanumericCharacters.Length)];
        }

        return new string(chars);
    }

    public static string InstanceName => Alphanumeric();
    public static string Username => Alphanumeric();
    public static string Email => Alphanumeric() + "@email.com";
    public static string Password => Alphanumeric();
    public static string ScopeName => Alphanumeric();
}
