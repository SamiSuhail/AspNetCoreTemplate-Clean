using MediatR;

namespace MyApp.Application.Interfaces.Commands.Auth.Registration.ConfirmUserRegistration;

public record ConfirmUserRegistrationRequest(string Code) : IRequest;
