using MediatR;

namespace MyApp.Presentation.Interfaces.Http.Commands.Infra.CreateInstance;

public record CreateInstanceRequest(string Name, bool IsCleanupEnabled) : IRequest;
