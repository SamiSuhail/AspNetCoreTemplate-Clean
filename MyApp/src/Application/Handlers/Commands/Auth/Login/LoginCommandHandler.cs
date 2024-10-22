using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Login;
using MyApp.Domain.Access.Scope;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;

namespace MyApp.Application.Handlers.Commands.Auth.Login;

public record LoginCommand(LoginRequest Request, string? InstanceName) : IRequest<LoginResponse>;

public class LoginCommandHandler(
    IScopedDbContext dbContext,
    IJwtGenerator jwtGenerator
    ) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var (request, instanceName) = command;
        var instanceId = await dbContext.GetInstanceId(instanceName, cancellationToken);

        var user = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Where(u => u.Username == request.Username && u.InstanceId == instanceId)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.PasswordHash,
                u.Email,
                u.IsEmailConfirmed,
                u.RefreshTokenVersion,
                Scopes = u.UserScopes.Select(us => us.Scope.Name)
                    .ToArray(),
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw LoginInvalidFailure.Exception();

        if (!user.IsEmailConfirmed)
            throw UserRegistrationNotConfirmedFailure.Exception();

        var correctPassword = request.Password.Verify(user.PasswordHash);
        if (!correctPassword)
            throw LoginInvalidFailure.Exception();

        var scopes = ScopeCollection.Create(user.Scopes);
        var accessToken = jwtGenerator.CreateAccessToken(user.Id, user.Username, user.Email, scopes);
        var refreshToken = jwtGenerator.CreateRefreshToken(user.Id, user.RefreshTokenVersion);

        return new(accessToken, refreshToken);
    }
}
