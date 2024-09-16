using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;

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
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw UserIdNotFoundFailure.Exception();

            user.SignOutOnAllDevices();
            await dbContext.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
