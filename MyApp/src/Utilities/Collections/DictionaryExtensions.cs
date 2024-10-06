namespace MyApp.Utilities.Collections;

public static class DictionaryExtensions
{
    public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key)
        => collection.TryGetValue(key, out var value)
            ? value
            : default;
}
