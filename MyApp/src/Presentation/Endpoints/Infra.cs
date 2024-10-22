using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyApp.Presentation.Interfaces.Http.Commands.Infra.CreateInstance;
using MyApp.Domain.Access.Scope;
using MyApp.Presentation.Endpoints.Core;

namespace MyApp.Presentation.Endpoints;
using CustomNoContent = Results<NoContent, CustomBadRequest, UnauthorizedHttpResult>;

public class Infra : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization(CustomScopes.InstanceWrite)
            .MapPost(CreateInstance, "instance/create");
    }

    public static async Task<CustomNoContent> 
        CreateInstance(
        [FromServices] ISender sender,
        [FromBody] CreateInstanceRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }
}
