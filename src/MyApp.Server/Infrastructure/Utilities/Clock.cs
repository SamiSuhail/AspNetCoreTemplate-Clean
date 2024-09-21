using MyApp.Application.Infrastructure.Abstractions;

namespace MyApp.Server.Infrastructure.Utilities;

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
