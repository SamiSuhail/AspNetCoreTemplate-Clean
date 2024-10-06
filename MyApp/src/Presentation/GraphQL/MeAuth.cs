using HotChocolate.Authorization;
using MediatR;
using MyApp.Application.Interfaces.Queries.Auth;

namespace MyApp.Presentation.GraphQL;

[Authorize]
[ExtendObjectType<Me>]
public class MeAuth
{
    public async Task<User> GetUser([Service] ISender sender, CancellationToken cancellationToken)
        => await sender.Send(new GetUserRequest(), cancellationToken);
}
