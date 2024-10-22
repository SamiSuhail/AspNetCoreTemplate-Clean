using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Handlers.Queries.Ping;
using MyApp.Presentation.Interfaces.Http.Queries.Ping;
using MyApp.Presentation.Endpoints.Core;


namespace MyApp.Presentation.Endpoints;

public class Ping : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(Default)
            .MapGet(Database, "/database");
    }

    [AllowAnonymous]
    public static Ok<Pong> Default()
    {
        return TypedResults.Ok(new Pong());
    }

    public static async Task<Results<Ok<Pong>, UnauthorizedHttpResult>> Database(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new PingDatabaseRequest(), cancellationToken);
        return TypedResults.Ok(response);
    }
}
