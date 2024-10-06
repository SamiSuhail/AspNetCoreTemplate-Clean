using MediatR;

namespace MyApp.Application.Interfaces.Commands.Auth.PasswordManagement.ForgotPassword;

public record ForgotPasswordRequest(string Email, string Username) : IRequest;
