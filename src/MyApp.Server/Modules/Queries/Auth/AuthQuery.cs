using MyApp.Server.Modules.Queries.Auth.Handlers;
using HotChocolate.Authorization;
using MediatR;

namespace MyApp.Server.Modules.Queries.Auth;

[Authorize]
[ExtendObjectType<MeQuery>]
public class AuthQuery
{
    public async Task<UserQuery> GetUser([Service] ISender sender, CancellationToken cancellationToken)
        => await sender.Send(new GetUserRequest(), cancellationToken);
}
