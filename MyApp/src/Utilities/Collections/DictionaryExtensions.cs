namespace MyApp.Utilities.Collections;

public static class DictionaryExtensions
{
    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> collection,
        TKey key,
        TValue? defaultValue = default)
        => collection.TryGetValue(key, out var value)
            ? value
            : defaultValue;
}
