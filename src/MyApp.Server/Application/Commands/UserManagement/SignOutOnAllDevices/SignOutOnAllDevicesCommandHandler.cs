using MediatR;
using MyApp.Server.Application.Utilities;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Abstractions.Auth;
using MyApp.Server.Infrastructure.Abstractions.Database;

namespace MyApp.Server.Application.Commands.UserManagement.SignOutOnAllDevices;

public record SignOutOnAllDevicesRequest() : IRequest;

public class SignOutOnAllDevicesCommandHandler(IUserContextAccessor userContextAccessor, IScopedDbContext dbContext) : IRequestHandler<SignOutOnAllDevicesRequest>
{
    public async Task Handle(SignOutOnAllDevicesRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, _) = userContextAccessor.User;

        await dbContext.WrapInTransaction(async () =>
        {
            var user = await dbContext.Set<UserEntity>()
                .FindUser(userId, cancellationToken);

            user.SignOutOnAllDevices();
            await dbContext.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
