using MediatR;

namespace MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ChangeEmail;

public record ChangeEmailRequest(string Email) : IRequest;
