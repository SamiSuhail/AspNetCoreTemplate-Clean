using Moq;

namespace MyApp.ApplicationIsolationTests.Core;

public class MockBag
{
    private readonly Dictionary<Type, Mock> mocks = [];

    public void Add(Type type, Mock mock)
    {
        mocks.Add(type, mock);
    }

    public void Add<TService>(Mock<TService> mock) where TService : class
    {
        mocks.Add(typeof(TService), mock);
    }

    public Mock<T> Get<T>() where T : class
    {
        return (Mock<T>) mocks[typeof(T)];
    }
}
