using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Modules.Commands.UserManagement.SignOutOnAllDevices;
using MyApp.Presentation.Endpoints.Core;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ChangeEmail;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ChangePassword;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

namespace MyApp.Presentation.Endpoints;
using CustomNoContent = Results<NoContent, BadRequest<ValidationProblemDetails>, UnauthorizedHttpResult>;

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

    public static async Task<CustomNoContent> 
        SignOutOnAllDevices(
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new SignOutOnAllDevicesRequest(), cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<CustomNoContent>
        ChangeEmail(
        [FromServices] ISender sender,
        [FromBody] ChangeEmailRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<CustomNoContent>
        ConfirmEmailChange(
        [FromServices] ISender sender,
        [FromBody] ConfirmEmailChangeRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<CustomNoContent>
        ChangePassword(
        [FromServices] ISender sender,
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<CustomNoContent>
        ConfirmPasswordChange(
        [FromServices] ISender sender,
        [FromBody] ConfirmPasswordChangeRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }
}
