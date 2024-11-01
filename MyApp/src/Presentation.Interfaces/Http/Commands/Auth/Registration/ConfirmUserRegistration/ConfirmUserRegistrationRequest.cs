using MediatR;

namespace MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ConfirmUserRegistration;

public record ConfirmUserRegistrationRequest(string Code) : IRequest;
