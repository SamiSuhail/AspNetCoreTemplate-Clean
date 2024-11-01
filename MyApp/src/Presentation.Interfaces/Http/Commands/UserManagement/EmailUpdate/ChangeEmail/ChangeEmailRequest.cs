using MediatR;

namespace MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ChangeEmail;

public record ChangeEmailRequest(string Email) : IRequest;
