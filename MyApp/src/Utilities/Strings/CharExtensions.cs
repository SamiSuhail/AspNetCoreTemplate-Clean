namespace MyApp.Utilities.Strings;

public static class CharExtensions
{
    public static bool IsAlphanumeric(this char c)
        => (c >= 'A' && c <= 'Z')
            || (c >= 'a' && c <= 'z')
            || (c >= '0' && c <= '9');
}
