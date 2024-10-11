namespace MyApp.Application.Infrastructure.Abstractions.Auth;

public interface IUnauthorizedRequestContextAccessor
{
    string InstanceName { get; }
}
