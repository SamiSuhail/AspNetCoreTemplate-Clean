namespace MyApp.Server.Infrastructure.Utilities;

public interface IClock
{
    public DateTime UtcNow { get; }
}

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
