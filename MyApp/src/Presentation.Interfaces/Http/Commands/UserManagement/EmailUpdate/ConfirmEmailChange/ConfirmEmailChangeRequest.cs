using MediatR;

namespace MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public record ConfirmEmailChangeRequest(string OldEmailCode, string NewEmailCode) : IRequest;
