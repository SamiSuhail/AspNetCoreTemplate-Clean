using MediatR;

namespace MyApp.Application.Interfaces.Commands.Auth.Registration.ResendConfirmation;

public record ResendConfirmationRequest(string Email) : IRequest;
