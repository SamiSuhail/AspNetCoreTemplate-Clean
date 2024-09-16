using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Commands.UserManagement.SignOutOnAllDevices;
using MyApp.Server.Presentation.Endpoints.Core;

namespace MyApp.Server.Presentation.Endpoints;

public class UserManagement : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(SignOutOnAllDevices, "sign-out-on-all-devices");
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task SignOutOnAllDevices(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new SignOutOnAllDevicesRequest(), cancellationToken);
    }
}
