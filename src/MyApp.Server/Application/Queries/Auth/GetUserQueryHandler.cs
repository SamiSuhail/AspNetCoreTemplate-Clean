using MyApp.Server.Domain.Auth.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;

namespace MyApp.Server.Application.Queries.Auth;

public record UserQuery(int Id, string Username, string Email, DateTime CreatedAt);
public record GetUserRequest() : IRequest<UserQuery>;

public class GetUserQueryHandler(IUserContextAccessor userReader, ITransientDbContext appDbContext) : IRequestHandler<GetUserRequest, UserQuery>
{
    public async Task<UserQuery> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var (id, username, email) = userReader.User;

        var createdAt = await appDbContext.Set<UserEntity>()
            .Where(u => u.Id == id)
            .Select(u => u.CreatedAt)
            .FirstAsync(cancellationToken);

        return new(id, username, email, createdAt);
    }
}
