using HotChocolate.Authorization;
using MediatR;

namespace MyApp.Server.Modules.Queries.Ping;

public class PingQuery
{
    public Pong Default()
        => new();

    [Authorize]
    public async Task<Pong> Database([Service] ISender sender, CancellationToken cancellationToken)
        => await sender.Send(new PingDatabaseRequest(), cancellationToken);
}
