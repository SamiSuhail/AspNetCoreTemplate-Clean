using MyApp.Infrastructure.Database;

namespace MyApp.Tests.Integration.Decorators;

public class GlobalSingletonDatabaseSeeder(IDatabaseSeeder inner) : IDatabaseSeeder
{
    private static SemaphoreSlim _semaphore = new(1, 1);
    private static bool _isInitialized = false;

    public async Task SeedDatabase()
    {
        await _semaphore.WaitAsync();

		try
		{
            if (_isInitialized)
                return;

            _isInitialized = true;
            await inner.SeedDatabase();
		}
		finally
        {
            _semaphore.Release();
        }
    }
}
