﻿using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Server.Modules.Commands.Auth.Login;

public record LoginRequest(
    string Username,
    string Password) : IRequest<LoginResponse>;

public record LoginResponse(string AccessToken);

public class LoginCommandHandler(IScopedDbContext dbContext, IJwtGenerator jwtGenerator) : IRequestHandler<LoginRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<UserEntity>()
            .Where(u => u.Username == request.Username)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.PasswordHash,
                u.Email,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw LoginInvalidFailure.Exception();

        var correctPassword = BC.EnhancedVerify(request.Password, user.PasswordHash);
        if (!correctPassword)
            throw LoginInvalidFailure.Exception();

        var accessToken = jwtGenerator.Create(user.Id, user.Username, user.Email);

        return new(accessToken);
    }
}