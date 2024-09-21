using MyApp.Application.Infrastructure.Abstractions;

namespace MyApp.Infrastructure.Utilities;

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
