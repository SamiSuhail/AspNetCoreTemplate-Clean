﻿using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Domain.Auth.User.Failures;
using MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation;
using MyApp.Server.Domain.UserManagement.PasswordChangeConfirmation.Failures;
using MyApp.Server.Infrastructure.Auth;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.Server.Application.Commands.UserManagement.PasswordUpdate.ChangePassword;

public record ChangePasswordRequest(string NewPassword) : IRequest;

public class ChangePasswordCommandHandler(IUserContextAccessor userContextAccessor, IScopedDbContext dbContext, IMessageProducer messageProducer) : IRequestHandler<ChangePasswordRequest>
{
    public async Task Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var (userId, username, email) = userContextAccessor.User;

        var userData = await dbContext.Set<UserEntity>()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.PasswordHash,
                u.PasswordChangeConfirmation,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw UserIdNotFoundFailure.Exception();

        var newPasswordChangeConfirmation = PasswordChangeConfirmationEntity.Create(userId, request.NewPassword);
        dbContext.Add(newPasswordChangeConfirmation);

        if (request.NewPassword.Verify(userData.PasswordHash))
            throw PasswordIsIdenticalFailure.Exception();

        if (userData.PasswordChangeConfirmation != null)
            dbContext.Remove(userData.PasswordChangeConfirmation);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new ChangePasswordMessage(username, email, newPasswordChangeConfirmation.Code), cancellationToken);
        }, cancellationToken);
    }
}