using MediatR;

namespace MyApp.Application.Interfaces.Commands.Auth.PasswordManagement.ResetPassword;

public record ResetPasswordRequest(string Code, string Password) : IRequest;
