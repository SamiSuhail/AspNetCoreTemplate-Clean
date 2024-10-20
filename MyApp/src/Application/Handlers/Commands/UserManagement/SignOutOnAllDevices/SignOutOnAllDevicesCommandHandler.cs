﻿using MediatR;
using MyApp.Domain.Auth.User;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Commands.UserManagement.SignOutOnAllDevices;
using MyApp.Application.Infrastructure;

namespace MyApp.Application.Handlers.Commands.UserManagement.SignOutOnAllDevices;

public class SignOutOnAllDevicesCommandHandler(IUserContextAccessor userContextAccessor, IScopedDbContext dbContext) : IRequestHandler<SignOutOnAllDevicesRequest>
{
    public async Task Handle(SignOutOnAllDevicesRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, _, _) = userContextAccessor.AccessToken;

        await dbContext.WrapInTransaction(async () =>
        {
            var user = await dbContext.Set<UserEntity>()
                .FindUser(userId, cancellationToken);

            user.SignOutOnAllDevices();
            await dbContext.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
