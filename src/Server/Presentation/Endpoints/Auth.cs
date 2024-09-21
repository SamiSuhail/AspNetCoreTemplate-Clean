using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Commands.Auth.Login;
using MyApp.Application.Commands.Auth.PasswordManagement.ForgotPassword;
using MyApp.Application.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Application.Commands.Auth.RefreshToken;
using MyApp.Application.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Application.Commands.Auth.Registration.Register;
using MyApp.Application.Commands.Auth.Registration.ResendConfirmation;
using MyApp.Server.Presentation.Endpoints.Core;
using MyApp.Server.Presentation.Startup.Filters;

namespace MyApp.Server.Presentation.Endpoints;

public class Auth : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .AllowAnonymous()
            .AddEndpointFilter<AnonymousOnlyFilter>()
            .MapPost(Register, "register")
            .MapPost(ResendConfirmation, "resend-confirmation")
            .MapPost(ConfirmUserRegistration, "confirm-user-registration")
            .MapPost(Login, "login")
            .MapPost(ForgotPassword, "forgot-password")
            .MapPost(ResetPassword, "reset-password");

        app.MapGroup(this)
            .AllowAnonymous()
            .MapPost(RefreshToken, "refresh-token");
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
    public async Task<IResult> ConfirmUserRegistration(
        [FromServices] ISender sender,
        [FromBody] ConfirmUserRegistrationRequest request,
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

    [ProducesResponseType<RefreshTokenResponse>(200)]
    [ProducesResponseType(400)]
    public async Task<RefreshTokenResponse> RefreshToken(
        [FromServices] ISender sender,
        [FromBody] RefreshTokenRequest request,
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
