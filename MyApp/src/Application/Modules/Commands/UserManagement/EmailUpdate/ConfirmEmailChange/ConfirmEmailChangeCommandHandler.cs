﻿using MediatR;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Infrastructure.Repositories.User;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.Shared.Confirmations;
using MyApp.Domain.UserManagement.EmailChangeConfirmation.Failures;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

namespace MyApp.Application.Modules.Commands.UserManagement.EmailUpdate.ConfirmEmailChange;

public class ConfirmEmailChangeCommandHandler(
    IUserContextAccessor userContextAccessor,
    IScopedDbContext dbContext
    ) : IRequestHandler<ConfirmEmailChangeRequest>
{
    public async Task Handle(ConfirmEmailChangeRequest request, CancellationToken cancellationToken)
    {
        var (userId, _, oldEmail, _) = userContextAccessor.UserData;
        var (oldEmailCode, newEmailCode) = request;

        var user = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Include(u => u.EmailChangeConfirmation)
            .FindUser(userId, cancellationToken);

        if (user.EmailChangeConfirmation == null)
            throw EmailChangeNotRequestedFailure.Exception();

        if (user.EmailChangeConfirmation.CreatedAt < DateTime.UtcNow.AddMinutes(-BaseConfirmationConstants.ExpirationTimeMinutes))
            throw EmailChangeExpiredFailure.Exception();

        if (oldEmailCode != user.EmailChangeConfirmation.OldEmailCode || newEmailCode != user.EmailChangeConfirmation.NewEmailCode)
            throw EmailChangeInvalidCodesFailure.Exception();

        var emailAlreadyInUse = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == user.EmailChangeConfirmation.NewEmail, cancellationToken);

        if (emailAlreadyInUse)
            throw UserConflictFailure.EmailException();

        user.ConfirmEmailChange();
        dbContext.Remove(user.EmailChangeConfirmation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
