using MediatR;

namespace MyApp.Presentation.Interfaces.Messaging;

public record ChangeEmailMessage(
    string Username,
    string OldEmail,
    string NewEmail,
    string OldEmailCode,
    string NewEmailCode) : IRequest;
