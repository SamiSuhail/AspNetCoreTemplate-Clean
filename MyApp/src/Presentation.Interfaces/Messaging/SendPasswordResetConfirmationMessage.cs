using MediatR;

namespace MyApp.Presentation.Interfaces.Messaging;

public record SendPasswordResetConfirmationMessage(string Username, string Email, string Code) : IRequest;