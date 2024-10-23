using MediatR;

namespace MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public record ConfirmPasswordChangeRequest(string Code, string Password) : IRequest;
