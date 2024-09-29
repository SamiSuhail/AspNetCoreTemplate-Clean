namespace MyApp.Application.Infrastructure.Abstractions.Database;

public interface IDatabasePinger
{
    Task<string> Ping(CancellationToken cancellationToken);
}
