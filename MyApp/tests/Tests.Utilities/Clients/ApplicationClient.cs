﻿using MyApp.Presentation.Interfaces.Http;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ResetPassword;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.RefreshToken;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ConfirmUserRegistration;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.Register;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ResendConfirmation;
using MyApp.Presentation.Interfaces.Http.Commands.Infra.CreateInstance;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ChangeEmail;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;
using MyApp.Presentation.Interfaces.Http.Queries.Ping;
using Refit;

namespace MyApp.Tests.Utilities.Clients;

public interface IApplicationAdminClient : IApplicationClient
{
    [Post("/api/infra/instance/create")]
    public Task<IApiResponse> CreateInstance(CreateInstanceRequest request);
}

public interface IApplicationClient
{
    [Get("/api/ping")]
    public Task<IApiResponse<Pong>> Ping();

    [Get("/api/ping/database")]
    public Task<IApiResponse<Pong>> PingDatabase();


    [Post("/api/auth/register")]
    public Task<IApiResponse> Register(RegisterRequest request);

    [Post("/api/auth/register")]
    public Task<IApiResponse> Register(RegisterRequest request, [Header(CustomHeaders.InstanceName)] string instanceName);

    [Post("/api/auth/resend-confirmation")]
    public Task<IApiResponse> ResendConfirmation(ResendConfirmationRequest request);

    [Post("/api/auth/confirm-user-registration")]
    public Task<IApiResponse> ConfirmUserRegistration(ConfirmUserRegistrationRequest request);

    [Post("/api/auth/login")]
    public Task<IApiResponse<LoginResponse>> Login(LoginRequest request);

    [Post("/api/auth/login")]
    public Task<IApiResponse<LoginResponse>> Login(LoginRequest request, [Header(CustomHeaders.InstanceName)] string instanceName);

    [Post("/api/auth/refresh-token")]
    public Task<IApiResponse<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest request);

    [Post("/api/auth/forgot-password")]
    public Task<IApiResponse> ForgotPassword(ForgotPasswordRequest request);

    [Post("/api/auth/reset-password")]
    public Task<IApiResponse> ResetPassword(ResetPasswordRequest request);


    [Post("/api/user-management/sign-out-on-all-devices")]
    public Task<IApiResponse> SignOutOnAllDevices();

    [Post("/api/user-management/change-email")]
    public Task<IApiResponse> ChangeEmail(ChangeEmailRequest request);

    [Post("/api/user-management/confirm-email-change")]
    public Task<IApiResponse> ConfirmEmailChange(ConfirmEmailChangeRequest request);

    [Post("/api/user-management/change-password")]
    public Task<IApiResponse> ChangePassword();

    [Post("/api/user-management/confirm-password-change")]
    public Task<IApiResponse> ConfirmPasswordChange(ConfirmPasswordChangeRequest request);
}
