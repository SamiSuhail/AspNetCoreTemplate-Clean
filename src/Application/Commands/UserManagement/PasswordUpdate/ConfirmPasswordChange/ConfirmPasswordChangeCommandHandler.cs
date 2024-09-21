﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Utilities;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation.Failures;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;

namespace MyApp.Application.Commands.UserManagement.PasswordUpdate.ConfirmPasswordChange;

public record ConfirmPasswordChangeRequest(string Code) : IRequest;

public class ConfirmPasswordChangeCommandHandler(IUserContextAccessor userContextAccessor, IScopedDbContext dbContext) : IRequestHandler<ConfirmPasswordChangeRequest>
{
    public async Task Handle(ConfirmPasswordChangeRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, _) = userContextAccessor.User;

        var user = await dbContext.Set<UserEntity>()
            .Include(u => u.PasswordChangeConfirmation)
            .FindUser(userId, cancellationToken);

        if (user.PasswordChangeConfirmation == null)
            throw PasswordChangeNotRequestedFailure.Exception();

        if (request.Code != user.PasswordChangeConfirmation.Code)
            throw PasswordChangeInvalidCodeFailure.Exception();

        var expirationTime = DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes);
        if (user.PasswordChangeConfirmation.CreatedAt < expirationTime)
            throw PasswordChangeExpiredFailure.Exception();

        user.ConfirmPasswordChange(user.PasswordChangeConfirmation.NewPasswordHash);
        dbContext.Remove(user.PasswordChangeConfirmation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}