using MyApp.Server.Infrastructure.Endpoints;
using MyApp.Server.Modules.Commands.Auth.ConfirmEmail;
using MyApp.Server.Modules.Commands.Auth.ForgotPassword;
using MyApp.Server.Modules.Commands.Auth.Login;
using MyApp.Server.Modules.Commands.Auth.Register;
using MyApp.Server.Modules.Commands.Auth.ResendConfirmation;
using MyApp.Server.Modules.Commands.Auth.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Server.Modules.Commands.Auth;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .AllowAnonymous()
            .MapPost(Register, "register")
            .MapPost(ResendConfirmation, "resend-confirmation")
            .MapPost(ConfirmEmail, "confirm-email")
            .MapPost(Login, "login")
            .MapPost(ForgotPassword, "forgot-password")
            .MapPost(ResetPassword, "reset-password");
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IResult> Register(
        [FromServices] ISender sender,
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return Results.NoContent();
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IResult> ResendConfirmation(
        [FromServices] ISender sender,
        [FromBody] ResendConfirmationRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return Results.NoContent();
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IResult> ConfirmEmail(
        [FromServices] ISender sender,
        [FromBody] ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return Results.NoContent();
    }

    [ProducesResponseType<LoginResponse>(200)]
    [ProducesResponseType(400)]
    public async Task<LoginResponse> Login(
        [FromServices] ISender sender,
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        return await sender.Send(request, cancellationToken);
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task ForgotPassword(
        [FromServices] ISender sender,
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
    }

    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task ResetPassword(
        [FromServices] ISender sender,
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
    }
}
