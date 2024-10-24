using MediatR;

namespace MyApp.Presentation.Interfaces.Messaging;

public record SendRegisterUserConfirmationMessage(string Username, string Email, string Code) : IRequest;