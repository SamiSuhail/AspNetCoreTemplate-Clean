using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Queries.Ping;

namespace MyApp.Application.Queries.Ping;

public class PingDatabaseQueryHandler(IDatabasePinger databasePinger) : IRequestHandler<PingDatabaseRequest, Pong>
{
    public async Task<Pong> Handle(PingDatabaseRequest request, CancellationToken cancellationToken)
    {
        var message = await databasePinger.Ping(cancellationToken);
        return new Pong(message);
    }
}
