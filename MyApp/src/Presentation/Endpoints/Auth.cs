using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Modules.Commands.Auth.Login;
using MyApp.Application.Modules.Commands.Auth.Registration.Register;
using MyApp.Presentation.Endpoints.Core;
using MyApp.Presentation.Interfaces.Http;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.RefreshToken;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.Register;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ResendConfirmation;
using MyApp.Presentation.Startup.Filters;

namespace MyApp.Presentation.Endpoints;
using CustomNoContent = Results<NoContent, CustomBadRequest>;

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

    public static async Task<CustomNoContent> 
        Register(
        [FromServices] ISender sender,
        [FromBody] RegisterRequest request,
        [FromHeader(Name = CustomHeaders.InstanceName)] string? instanceName,
        CancellationToken cancellationToken)
    {
        await sender.Send(new RegisterCommand(request, instanceName), cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<CustomNoContent>
        ResendConfirmation(
        [FromServices] ISender sender,
        [FromBody] ResendConfirmationRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<CustomNoContent>
        ConfirmUserRegistration(
        [FromServices] ISender sender,
        [FromBody] ConfirmUserRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<Results<Ok<LoginResponse>, CustomBadRequest>> 
        Login(
        [FromServices] ISender sender,
        [FromBody] LoginRequest request,
        [FromHeader(Name = CustomHeaders.InstanceName)] string? instanceName,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new LoginCommand(request, instanceName), cancellationToken);
        return TypedResults.Ok(response);
    }

    public static async Task<Results<Ok<RefreshTokenResponse>, CustomBadRequest>> 
        RefreshToken(
        [FromServices] ISender sender,
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(response);
    }

    public static async Task<CustomNoContent>
        ForgotPassword(
        [FromServices] ISender sender,
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }

    public static async Task<CustomNoContent>
        ResetPassword(
        [FromServices] ISender sender,
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(request, cancellationToken);
        return TypedResults.NoContent();
    }
}
