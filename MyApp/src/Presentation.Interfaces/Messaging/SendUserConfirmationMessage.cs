using MediatR;

namespace MyApp.Presentation.Interfaces.Messaging;

public record SendUserConfirmationMessage(string Username, string Email, string Code) : IRequest;