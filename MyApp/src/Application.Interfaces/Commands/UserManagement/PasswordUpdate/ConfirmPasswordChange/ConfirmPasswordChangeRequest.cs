using MediatR;

namespace MyApp.Application.Interfaces.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public record ConfirmPasswordChangeRequest(string Code) : IRequest;
