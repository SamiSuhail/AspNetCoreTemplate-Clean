using MediatR;

namespace MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ResendConfirmation;

public record ResendConfirmationRequest(string Email) : IRequest;
