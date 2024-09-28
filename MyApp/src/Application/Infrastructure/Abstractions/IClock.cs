namespace MyApp.Application.Infrastructure.Abstractions;
public interface IClock
{
    public DateTime UtcNow { get; }
}
