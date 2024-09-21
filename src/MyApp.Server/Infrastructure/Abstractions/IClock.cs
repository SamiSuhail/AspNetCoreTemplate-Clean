namespace MyApp.Server.Infrastructure.Abstractions;
public interface IClock
{
    public DateTime UtcNow { get; }
}
