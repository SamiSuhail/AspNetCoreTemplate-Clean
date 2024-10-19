namespace MyApp.Utilities.Collections;

public static class IDictionaryExtensions
{
    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue>? collection,
        TKey key,
        TValue? defaultValue = default)
        => collection?.TryGetValue(key, out var value) == true
            ? value
            : defaultValue;
}
