﻿using MyApp.Domain.Auth.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;

namespace MyApp.Application.Queries.Auth;

public record UserQuery(int Id, string Username, string Email, DateTime CreatedAt);
public record GetUserRequest() : IRequest<UserQuery>;

public class GetUserQueryHandler(IUserContextAccessor userReader, ITransientDbContext appDbContext) : IRequestHandler<GetUserRequest, UserQuery>
{
    public async Task<UserQuery> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var (id, username, email) = userReader.User;

        var user = await appDbContext.Set<UserEntity>()
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.CreatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw UserIdNotFoundFailure.Exception();

        return new(id, username, email, user.CreatedAt);
    }
}