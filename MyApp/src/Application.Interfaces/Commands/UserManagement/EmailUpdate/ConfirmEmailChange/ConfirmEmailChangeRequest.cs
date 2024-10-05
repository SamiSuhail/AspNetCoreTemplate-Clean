using MediatR;

namespace MyApp.Application.Interfaces.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public record ConfirmEmailChangeRequest(string OldEmailCode, string NewEmailCode) : IRequest;
