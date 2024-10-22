using HotChocolate.Authorization;
using MediatR;
using MyApp.Application.Handlers.Queries.Auth;
using MyApp.Presentation.Interfaces.Http.Queries.Auth;

namespace MyApp.Presentation.GraphQL;
#pragma warning disable CA1822 // Mark members as static

[Authorize]
[ExtendObjectType<Me>]
public class MeAuth
{
    public async Task<User> GetUser([Service] ISender sender, CancellationToken cancellationToken)
        => await sender.Send(new GetUserRequest(), cancellationToken);
}
