namespace MyApp.Server.Utilities;

public static class StringExtensions
{
    public static string RemoveWhitespace(this string s)
        => s.ReplaceWhitespace(string.Empty);

    public static string ReplaceWhitespace(this string s, string newValue)
        => s.ReplaceMany(newValue, " ", "\r", "\n", "\t");

    public static string RemoveMany(this string s, params string[] oldValues)
        => s.ReplaceMany(string.Empty, oldValues);

    public static string ReplaceMany(this string s, string newVal, params string[] oldValues)
    {
        var temp = s.Split(oldValues, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(newVal, temp);
    }
}
