using System.Runtime.CompilerServices;

namespace MyApp.Utilities.Tasks;

public static class IAsyncEnumerableExtensions
{
    public static async Task<T?> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> inputs)
        => await inputs.FirstOrDefaultAsync(_ => true);

    public static async Task<T?> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> inputs, Func<T, bool> predicate)
    {
        await foreach (var input in inputs)
            if (predicate(input))
                return input;

        return default;
    }

    public static async Task EnumerateAsync<T>(this IAsyncEnumerable<T> inputs)
    {
        await foreach (var _ in inputs)
        { }
    }

    public static async Task WhenAllParallelized<TInput>(
        this IEnumerable<TInput> inputs,
        Func<TInput, Task> action,
        int parallelizationCount = 10)
    {
        await inputs.WhenAllParallelized(async (input, _) =>
        {
            await action(input);
        }, parallelizationCount);
    }

    public static async Task WhenAllParallelized<T>(
        this IEnumerable<T> inputs,
        Func<T, CancellationToken, Task> action,
        int parallelizationCount = 10,
        CancellationToken cancellationToken = default)
    {
        await inputs.WhenAllParallelized(async (input, ct) =>
            {
                await action(input, ct);
                return 0;
            }, parallelizationCount, cancellationToken)
            .EnumerateAsync();
    }

    public static async IAsyncEnumerable<TResult> WhenAllParallelized<TInput, TResult>(
        this IEnumerable<TInput> inputs,
        Func<TInput, Task<TResult>> action,
        int parallelizationCount = 10)
    {
        var results = inputs.WhenAllParallelized(async (input, _) => await action(input), parallelizationCount);
        await foreach (var result in results)
            yield return result;
    }

    public static async IAsyncEnumerable<TResult> WhenAllParallelized<TInput, TResult>(
        this IEnumerable<TInput> inputs,
        Func<TInput, CancellationToken, Task<TResult>> action,
        int parallelizationCount = 10,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var tasks = new Task<TResult>[parallelizationCount];

        var counter = 0;
        foreach (var input in inputs)
        {
            if(cancellationToken.IsCancellationRequested)
                yield break;

            if (counter < parallelizationCount)
            {
                tasks[counter] = action(input, cancellationToken);
                counter++;
                continue;
            }

            var task = await Task.WhenAny(tasks);
            var index = Array.IndexOf(tasks, task);
            tasks[index] = action(input, cancellationToken);
            yield return await task;
        }

        if (counter == 0)
            yield break;

        var remainingResults = await Task.WhenAll(tasks.Where(t => t != null));
        foreach (var result in remainingResults)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return result;
        }
    }
}
