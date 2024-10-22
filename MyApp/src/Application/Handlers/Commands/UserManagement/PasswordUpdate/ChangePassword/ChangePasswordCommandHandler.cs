using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Presentation.Interfaces.Http.Commands.UserManagement.PasswordUpdate.ChangePassword;
using MyApp.Domain.Auth.User;
using MyApp.Domain.Auth.User.Failures;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation;
using MyApp.Domain.UserManagement.PasswordChangeConfirmation.Failures;

namespace MyApp.Application.Handlers.Commands.UserManagement.PasswordUpdate.ChangePassword;

public class ChangePasswordCommandHandler(IUserContextAccessor userContextAccessor, IScopedDbContext dbContext, IMessageProducer messageProducer) : IRequestHandler<ChangePasswordRequest>
{
    public async Task Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var (userId, username, email, _) = userContextAccessor.UserData;

        var userData = await dbContext.Set<UserEntity>()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.PasswordHash,
                u.PasswordChangeConfirmation,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw UserNotFoundFailure.Exception();

        if (request.NewPassword.Verify(userData.PasswordHash))
            throw PasswordIsIdenticalFailure.Exception();

        var newPasswordChangeConfirmation = PasswordChangeConfirmationEntity.Create(userId, request.NewPassword);
        dbContext.Add(newPasswordChangeConfirmation);

        if (userData.PasswordChangeConfirmation != null)
            dbContext.Remove(userData.PasswordChangeConfirmation);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new ChangePasswordMessage(username, email, newPasswordChangeConfirmation.Code), cancellationToken);
        }, cancellationToken);
    }
}
