namespace MyApp.Utilities.Collections;

public static class IEnumerableExtensions
{
    public static bool NotContains<T>(this IEnumerable<T> items, T value, IEqualityComparer<T> comparer)
        => !items.Contains(value, comparer);
    public static bool NotContains<T>(this IEnumerable<T> items, T value)
        => !items.Contains(value);

    public static bool None<T>(this IEnumerable<T> items)
        => !items.Any();
    public static bool None<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        => !items.Any(predicate);
}
