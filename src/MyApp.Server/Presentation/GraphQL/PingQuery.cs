using HotChocolate.Authorization;
using MediatR;
using MyApp.Application.Queries.Ping;

namespace MyApp.Server.Presentation.GraphQL;

public class PingQuery
{
    public Pong Default()
        => new();

    [Authorize]
    public async Task<Pong> Database([Service] ISender sender, CancellationToken cancellationToken)
        => await sender.Send(new PingDatabaseRequest(), cancellationToken);
}
