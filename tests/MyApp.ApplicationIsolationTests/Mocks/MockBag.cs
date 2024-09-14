namespace MyApp.ApplicationIsolationTests.Mocks;

public static class MockBag
{
    private static readonly Dictionary<Type, Mock> mocks = [];

    public static void Add(Type type, Mock mock)
    {
        mocks.Add(type, mock);
    }

    public static void Add<TService>(Mock<TService> mock) where TService : class
    {
        mocks.Add(typeof(TService), mock);
    }

    public static Mock<T> Get<T>() where T : class
    {
        return (Mock<T>)mocks[typeof(T)];
    }
}
