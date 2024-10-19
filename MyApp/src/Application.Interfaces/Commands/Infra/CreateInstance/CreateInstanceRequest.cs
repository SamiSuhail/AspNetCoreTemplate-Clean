using MediatR;

namespace MyApp.Application.Interfaces.Commands.Infra.CreateInstance;

public record CreateInstanceRequest(string Name, bool IsCleanupEnabled) : IRequest;
