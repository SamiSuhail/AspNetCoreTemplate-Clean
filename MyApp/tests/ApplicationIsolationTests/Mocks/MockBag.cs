namespace MyApp.ApplicationIsolationTests.Mocks;

public class MockBag
{
    private readonly Dictionary<Type, Mock> _mocks = [];

    public void Add(Type type, Mock mock)
    {
        _mocks.Add(type, mock);
    }

    public void Add<TService>(Mock<TService> mock) where TService : class
    {
        _mocks.Add(typeof(TService), mock);
    }

    public Mock<T> Get<T>() where T : class
    {
        return (Mock<T>)_mocks[typeof(T)];
    }

    public void Reset()
    {
        foreach (var (_, mock) in _mocks)
            mock.Reset();
    }

    public void VerifyAll()
    {
        foreach (var (_, mock) in _mocks)
            mock.VerifyAll();
    }
}
