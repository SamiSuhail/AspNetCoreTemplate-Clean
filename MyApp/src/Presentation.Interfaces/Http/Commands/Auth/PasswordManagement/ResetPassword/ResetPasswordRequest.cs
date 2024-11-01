using MediatR;

namespace MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ResetPassword;

public record ResetPasswordRequest(string Code, string Password) : IRequest;
