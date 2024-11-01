using HotChocolate.Authorization;
using MyApp.Application.Modules.Queries.Ping;
using MyApp.Presentation.Interfaces.Http.Queries.Ping;

namespace MyApp.Presentation.GraphQL;
#pragma warning disable CA1822 // Mark members as static

public class Ping
{
    public Pong Default()
        => new();

    [Authorize]
    public async Task<Pong> Database([Service] ISender sender, CancellationToken cancellationToken)
        => await sender.Send(new PingDatabaseRequest(), cancellationToken);
}
