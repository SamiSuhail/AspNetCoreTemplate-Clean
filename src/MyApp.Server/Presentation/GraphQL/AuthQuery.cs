using HotChocolate.Authorization;
using MediatR;
using MyApp.Server.Application.Queries.Auth;

namespace MyApp.Server.Presentation.GraphQL;

[Authorize]
[ExtendObjectType<MeQuery>]
public class AuthQuery
{
    public async Task<UserQuery> GetUser([Service] ISender sender, CancellationToken cancellationToken)
        => await sender.Send(new GetUserRequest(), cancellationToken);
}
