namespace MyApp.Tests.Utilities.Core;

/// <summary>
/// This class is used to ensure a task is only ran once even when accessed by multiple threads in parallel.
/// Those threads need to share the instance of <see cref="AsyncLazy"/>.
/// </summary>
/// <param name="func">A function returning the task that needs to be ran.</param>
public class AsyncLazy(Func<Task> func)
{
    private readonly Lazy<Task> _lazy = new(() => Task.Run(func));

    public Task Value => _lazy.Value;
}
