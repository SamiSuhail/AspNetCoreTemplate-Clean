using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Interfaces.Commands.Infra.CreateInstance;
using MyApp.Domain.Access.Scope;
using MyApp.Presentation.Endpoints.Core;

namespace MyApp.Presentation.Endpoints;

public class Infra : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(CustomScopes.InstanceWrite)
            .MapPost(CreateInstance, "instance/create");
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    public async Task CreateInstance(
        [FromServices] ISender sender,
        [FromBody] CreateInstanceRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
    }
}
