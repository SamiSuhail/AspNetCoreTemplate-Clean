using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Commands.UserManagement.EmailUpdate.ChangeEmail;
using MyApp.Application.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Application.Commands.UserManagement.PasswordUpdate.ChangePassword;
using MyApp.Application.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Application.Commands.UserManagement.SignOutOnAllDevices;
using MyApp.Presentation.Endpoints.Core;

namespace MyApp.Presentation.Endpoints;

public class UserManagement : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(SignOutOnAllDevices, "sign-out-on-all-devices")
            .MapPost(ChangeEmail, "change-email")
            .MapPost(ConfirmEmailChange, "confirm-email-change")
            .MapPost(ChangePassword, "change-password")
            .MapPost(ConfirmPasswordChange, "confirm-password-change");
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public static async Task SignOutOnAllDevices(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new SignOutOnAllDevicesRequest(), cancellationToken);
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public static async Task ChangeEmail(
        [FromServices] ISender sender,
        [FromBody] ChangeEmailRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public static async Task ConfirmEmailChange(
        [FromServices] ISender sender,
        [FromBody] ConfirmEmailChangeRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public static async Task ChangePassword(
        [FromServices] ISender sender,
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public static async Task ConfirmPasswordChange(
        [FromServices] ISender sender,
        [FromBody] ConfirmPasswordChangeRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
    }
}
