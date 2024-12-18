﻿using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Infrastructure.Repositories.User;
using MyApp.Domain.Auth.User;

namespace MyApp.Application.Modules.Commands.UserManagement.SignOutOnAllDevices;

public record SignOutOnAllDevicesRequest() : IRequest;
public class SignOutOnAllDevicesCommandHandler(IUserContextAccessor userContextAccessor, IScopedDbContext dbContext) : IRequestHandler<SignOutOnAllDevicesRequest>
{
    public async Task Handle(SignOutOnAllDevicesRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, _, _) = userContextAccessor.UserData;

        await dbContext.WrapInTransaction(async () =>
        {
            var user = await dbContext.Set<UserEntity>()
                .FindUser(userId, cancellationToken);

            user.SignOutOnAllDevices();
            await dbContext.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
