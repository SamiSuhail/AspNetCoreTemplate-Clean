using HotChocolate.Authorization;
using MediatR;
using MyApp.Application.Interfaces.Queries.Ping;

namespace MyApp.Presentation.GraphQL;

public class Ping
{
    public Pong Default()
        => new();

    [Authorize]
    public async Task<Pong> Database([Service] ISender sender, CancellationToken cancellationToken)
        => await sender.Send(new PingDatabaseRequest(), cancellationToken);
}
