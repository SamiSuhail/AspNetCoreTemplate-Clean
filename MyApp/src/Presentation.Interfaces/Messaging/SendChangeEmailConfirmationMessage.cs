using MediatR;

namespace MyApp.Presentation.Interfaces.Messaging;

public record SendChangeEmailConfirmationMessage(
    string Username,
    string OldEmail,
    string NewEmail,
    string OldEmailCode,
    string NewEmailCode) : IRequest;
