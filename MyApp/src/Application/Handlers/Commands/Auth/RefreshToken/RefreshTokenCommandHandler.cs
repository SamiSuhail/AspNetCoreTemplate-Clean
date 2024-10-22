using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Access.Scope;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.RefreshToken;

namespace MyApp.Application.Handlers.Commands.Auth.RefreshToken;

public class RefreshTokenCommandHandler(
    IJwtReader jwtReader,
    IJwtGenerator jwtGenerator,
    IScopedDbContext dbContext
    ) : IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var (userId, username, email, scopes) = jwtReader.ReadAccessToken(request.AccessToken)
            ?? throw UserSessionCouldNotBeRefreshedFailure.Exception();
        var (refreshTokenUserId, refreshTokenVersion) = jwtReader.ReadRefreshToken(request.RefreshToken)
            ?? throw UserSessionCouldNotBeRefreshedFailure.Exception();

        if (userId != refreshTokenUserId)
            throw UserSessionCouldNotBeRefreshedFailure.Exception();

        var user = await dbContext.Set<UserEntity>()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.RefreshTokenVersion,
                ValidScopes = u.UserScopes.Where(us => scopes.Contains(us.Scope.Name))
                    .Select(us => us.Scope.Name)
                    .ToArray()
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw UserNotFoundFailure.Exception();

        if (user.RefreshTokenVersion != refreshTokenVersion)
            throw UserSessionCouldNotBeRefreshedFailure.Exception();

        var newAccessToken = jwtGenerator.CreateAccessToken(userId, username, email, ScopeCollection.Create(user.ValidScopes));
        var newRefreshToken = jwtGenerator.CreateRefreshToken(userId, refreshTokenVersion);

        return new(newAccessToken, newRefreshToken);
    }
}
