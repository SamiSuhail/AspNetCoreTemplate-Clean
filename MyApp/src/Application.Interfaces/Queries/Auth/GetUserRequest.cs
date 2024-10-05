using MediatR;

namespace MyApp.Application.Interfaces.Queries.Auth;

public record GetUserRequest() : IRequest<User>;
