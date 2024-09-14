namespace MyApp.ApplicationIsolationTests.Mocks;

public static class MockBag
{
    private static readonly Dictionary<Type, Mock> _mocks = [];

    public static void Add(Type type, Mock mock)
    {
        _mocks.Add(type, mock);
    }

    public static void Add<TService>(Mock<TService> mock) where TService : class
    {
        _mocks.Add(typeof(TService), mock);
    }

    public static Mock<T> Get<T>() where T : class
    {
        return (Mock<T>)_mocks[typeof(T)];
    }

    public static void Reset()
    {
        foreach (var (_, mock) in _mocks)
            mock.Reset();
    }
}
