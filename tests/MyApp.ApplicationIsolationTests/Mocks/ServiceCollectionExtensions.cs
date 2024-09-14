namespace MyApp.ApplicationIsolationTests.Mocks;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ReplaceStandardMocks(this IServiceCollection services)
    {
        var interfaces = services
            .Where(t => t.ServiceType.Name.EndsWith("EmailNotifier") && t.ServiceType.IsInterface)
            .ToArray();

        foreach (var descriptor in interfaces)
        {
            var serviceType = descriptor.ServiceType;
            var mockType = typeof(Mock<>).MakeGenericType(serviceType);
            var mockInstance = Activator.CreateInstance(mockType, MockBehavior.Strict)
                ?? throw new InvalidOperationException($"Could not active type {serviceType.Name} while replacing standard mocks.");
            var mock = (Mock)mockInstance;

            services.Remove(descriptor);
            services.AddSingleton(serviceType, mock.Object);
            MockBag.Add(serviceType, mock);
        }

        return services;
    }

    public static IServiceCollection ReplaceWithMock<TService>(
        this IServiceCollection services,
        MockBehavior mockBehavior = MockBehavior.Strict) 
        where TService : class
    {
        var mock = new Mock<TService>(mockBehavior);
        MockBag.Add(mock);
        services.ReplaceService(mock.Object);
        return services;
    }

    public static void ReplaceService<T>(this IServiceCollection services, T service) where T : class
    {
        var serviceType = typeof(T);
        var descriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == serviceType)
            ?? throw new Exception($"Unable to remove service of type {serviceType.Name} from service collection. No such service was found.");
        services.Remove(descriptor);
        services.AddSingleton(service);
    }
}
