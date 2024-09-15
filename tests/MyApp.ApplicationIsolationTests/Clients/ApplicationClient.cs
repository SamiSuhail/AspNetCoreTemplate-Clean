using MyApp.Server.Application.Commands.Auth.Login;
using MyApp.Server.Application.Commands.Auth.PasswordManagement.ForgotPassword;
using MyApp.Server.Application.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Server.Application.Commands.Auth.Registration.ConfirmEmail;
using MyApp.Server.Application.Commands.Auth.Registration.Register;
using MyApp.Server.Application.Commands.Auth.Registration.ResendConfirmation;
using MyApp.Server.Application.Queries.Ping;

namespace MyApp.ApplicationIsolationTests.Clients;

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
