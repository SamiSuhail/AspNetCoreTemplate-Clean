using MediatR;

namespace MyApp.Application.Interfaces.Queries.Ping;

public record PingDatabaseRequest() : IRequest<Pong>;