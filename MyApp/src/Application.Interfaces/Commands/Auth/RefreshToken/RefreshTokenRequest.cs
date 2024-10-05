using MediatR;

namespace MyApp.Application.Interfaces.Commands.Auth.RefreshToken;

public record RefreshTokenRequest(string RefreshToken) : IRequest<RefreshTokenResponse>;
