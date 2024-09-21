using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Database;

namespace MyApp.Application.Queries.Ping;

public record PingDatabaseRequest() : IRequest<Pong>;

public class PingDatabaseQueryHandler(IDatabasePinger databasePinger) : IRequestHandler<PingDatabaseRequest, Pong>
{
    public async Task<Pong> Handle(PingDatabaseRequest request, CancellationToken cancellationToken)
    {
        var message = await databasePinger.Ping(cancellationToken);
        return new Pong(message);
    }
}
