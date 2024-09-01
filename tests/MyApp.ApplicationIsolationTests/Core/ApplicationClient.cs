using MyApp.Server.Modules.Commands.Auth.ConfirmEmail;
using MyApp.Server.Modules.Commands.Auth.ForgotPassword;
using MyApp.Server.Modules.Commands.Auth.Login;
using MyApp.Server.Modules.Commands.Auth.Register;
using MyApp.Server.Modules.Commands.Auth.ResendConfirmation;
using MyApp.Server.Modules.Commands.Auth.ResetPassword;
using MyApp.Server.Modules.Queries.Ping;
using Refit;

namespace MyApp.ApplicationIsolationTests.Core;

public interface IApplicationClient
{
    [Get("/api/ping")]
    public Task<IApiResponse<Pong>> Ping();

    [Get("/api/ping/database")]
    public Task<IApiResponse<Pong>> PingDatabase();


    [Post("/api/auth/register")]
    public Task<IApiResponse> Register(RegisterRequest request);

    [Post("/api/auth/resend-confirmation")]
    public Task<IApiResponse> ResendConfirmation(ResendConfirmationRequest request);

    [Post("/api/auth/confirm-email")]
    public Task<IApiResponse> ConfirmEmail(ConfirmEmailRequest request);

    [Post("/api/auth/login")]
    public Task<IApiResponse<LoginResponse>> Login(LoginRequest request);

    [Post("/api/auth/forgot-password")]
    public Task<IApiResponse> ForgotPassword(ForgotPasswordRequest request);

    [Post("/api/auth/reset-password")]
    public Task<IApiResponse> ResetPassword(ResetPasswordRequest request);
}
