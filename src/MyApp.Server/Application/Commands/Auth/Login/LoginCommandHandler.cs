using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;

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
            throw UserRegistrationNotConfirmedFailure.Exception();

        var correctPassword = request.Password.Verify(user.PasswordHash);
        if (!correctPassword)
            throw LoginInvalidFailure.Exception();

        var accessToken = jwtGenerator.CreateAccessToken(user.Id, user.Username, user.Email);
        var refreshToken = jwtGenerator.CreateRefreshToken(user.Id, user.Username, user.Email, user.RefreshTokenVersion);

        return new(accessToken, refreshToken);
    }
}
