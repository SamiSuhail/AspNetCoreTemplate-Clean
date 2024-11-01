using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Presentation.Interfaces.Http.Queries.Auth;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;

namespace MyApp.Application.Modules.Queries.Auth;

public record GetUserRequest() : IRequest<User>;
public class GetUserQueryHandler(IUserContextAccessor userContextAccessor, ITransientDbContext appDbContext) : IRequestHandler<GetUserRequest, User>
{
    public async Task<User> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var (id, username, email, _) = userContextAccessor.UserData;

        var user = await appDbContext.Set<UserEntity>()
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.CreatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw UserNotFoundFailure.Exception();

        return new(id, username, email, user.CreatedAt);
    }
}
