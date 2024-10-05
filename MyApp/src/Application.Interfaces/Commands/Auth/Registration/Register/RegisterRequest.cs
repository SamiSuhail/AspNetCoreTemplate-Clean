using MediatR;

namespace MyApp.Application.Interfaces.Commands.Auth.Registration.Register;

public record RegisterRequest(
    string Email,
    string Username,
    string Password) : IRequest;
