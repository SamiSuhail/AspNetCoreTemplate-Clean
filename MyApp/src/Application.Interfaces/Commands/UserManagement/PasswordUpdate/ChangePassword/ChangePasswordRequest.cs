using MediatR;

namespace MyApp.Application.Interfaces.Commands.UserManagement.PasswordUpdate.ChangePassword;

public record ChangePasswordRequest(string NewPassword) : IRequest;
