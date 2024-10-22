using MediatR;

namespace MyApp.Presentation.Interfaces.Http.Commands.Auth.RefreshToken;

public record RefreshTokenRequest(string AccessToken, string RefreshToken) : IRequest<RefreshTokenResponse>;
