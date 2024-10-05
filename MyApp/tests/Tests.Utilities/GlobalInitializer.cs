namespace Tests.Utilities;

public static class GlobalInitializer
{
    public static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static bool _isInitialized = false;

    public static async Task InitializeAsync(Func<Task> action)
    {
        await _semaphore.WaitAsync();

		try
		{
            if (_isInitialized)
                return;

            _isInitialized = true;

            await action();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
