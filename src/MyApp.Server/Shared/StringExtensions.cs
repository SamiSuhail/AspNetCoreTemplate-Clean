using System.Text;

namespace MyApp.Server.Shared;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
        => value == null || value.Length == 0;

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

    public static string PascalToKebabCase(this string txt)
    {
        if (txt.Length == 0) return string.Empty;

        var sb = new StringBuilder();

        for (var i = 0; i < txt.Length; i++)
        {
            if (char.IsLower(txt[i])) // if current char is already lowercase
            {
                sb.Append(txt[i]);
            }
            else if (i == 0) // if current char is the first char
            {
                sb.Append(char.ToLower(txt[i]));
            }
            else if (char.IsDigit(txt[i]) && !char.IsDigit(txt[i - 1])) // if current char is a number and the previous is not
            {
                sb.Append('-');
                sb.Append(txt[i]);
            }
            else if (char.IsDigit(txt[i])) // if current char is a number and previous is
            {
                sb.Append(txt[i]);
            }
            else if (char.IsLower(txt[i - 1])) // if current char is upper and previous char is lower
            {
                sb.Append('-');
                sb.Append(char.ToLower(txt[i]));
            }
            else if (i + 1 == txt.Length || char.IsUpper(txt[i + 1])) // if current char is upper and next char doesn't exist or is upper
            {
                sb.Append(char.ToLower(txt[i]));
            }
            else // if current char is upper and next char is lower
            {
                sb.Append('-');
                sb.Append(char.ToLower(txt[i]));
            }
        }
        return sb.ToString();
    }
}
