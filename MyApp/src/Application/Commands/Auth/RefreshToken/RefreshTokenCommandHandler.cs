using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Commands.Auth.RefreshToken;

namespace MyApp.Application.Commands.Auth.RefreshToken;

public class RefreshTokenCommandHandler(
    IJwtReader jwtReader,
    IJwtGenerator jwtGenerator,
    IScopedDbContext dbContext
    ) : IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var (userId, username, email, refreshTokenVersion) = jwtReader.ReadRefreshToken(request.RefreshToken)
            ?? throw UserSessionCouldNotBeRefreshedFailure.Exception();

        var user = await dbContext.Set<UserEntity>()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.RefreshTokenVersion,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw UserSessionCouldNotBeRefreshedFailure.Exception();

        if (user.RefreshTokenVersion != refreshTokenVersion)
            throw UserSessionCouldNotBeRefreshedFailure.Exception();

        var newAccessToken = jwtGenerator.CreateAccessToken(userId, username, email);
        var newRefreshToken = jwtGenerator.CreateRefreshToken(userId, username, email, refreshTokenVersion);

        return new(newAccessToken, newRefreshToken);
    }
}
