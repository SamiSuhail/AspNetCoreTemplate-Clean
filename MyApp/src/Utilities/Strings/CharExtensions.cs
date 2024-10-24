namespace MyApp.Utilities.Strings;

public static class CharExtensions
{
    public static bool IsAlphanumeric(this char c)
        => c.IsUppercase()
            || c.IsLowercase()
            || c.IsNumber();

    public static bool IsUppercase(this char c)
        => (c >= 'A' && c <= 'Z');

    public static bool IsLowercase(this char c)
        => (c >= 'a' && c <= 'z');

    public static bool IsNumber(this char c)
        => (c >= '0' && c <= '9');
}
