using MediatR;

namespace MyApp.Presentation.Interfaces.Messaging;

public record PasswordResetSendConfirmationMessage(string Username, string Email, string Code) : IRequest;