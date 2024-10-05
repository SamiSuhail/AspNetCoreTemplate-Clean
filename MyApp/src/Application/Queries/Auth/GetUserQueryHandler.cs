using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Queries.Auth;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;

namespace MyApp.Application.Queries.Auth;

public class GetUserQueryHandler(IUserContextAccessor userReader, ITransientDbContext appDbContext) : IRequestHandler<GetUserRequest, User>
{
    public async Task<User> Handle(GetUserRequest request, CancellationToken cancellationToken)
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
