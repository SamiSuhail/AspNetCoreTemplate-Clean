﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Queries.Ping;
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
    public Pong Default()
    {
        return new Pong();
    }

    [ProducesResponseType(401)]
    public async Task<Pong> Database(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new PingDatabaseRequest(), cancellationToken);
    }
}