using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Application.Commands.Auth.Login;

public record LoginRequest(
    string Username,
    string Password) : IRequest<LoginResponse>;

public record LoginResponse(string AccessToken, string RefreshToken);

public class LoginCommandHandler(IScopedDbContext dbContext, IJwtGenerator jwtGenerator) : IRequestHandler<LoginRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Where(u => u.Username == request.Username)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.PasswordHash,
                u.Email,
                u.IsEmailConfirmed,
                u.RefreshTokenVersion,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw LoginInvalidFailure.Exception();

        if (!user.IsEmailConfirmed)
            throw EmailNotConfirmedFailure.Exception();

        var correctPassword = BC.EnhancedVerify(request.Password, user.PasswordHash);
        if (!correctPassword)
            throw LoginInvalidFailure.Exception();

        var accessToken = jwtGenerator.CreateAccessToken(user.Id, user.Username, user.Email);
        var refreshToken = jwtGenerator.CreateRefreshToken(user.Id, user.Username, user.Email, user.RefreshTokenVersion);

        return new(accessToken, refreshToken);
    }
}
