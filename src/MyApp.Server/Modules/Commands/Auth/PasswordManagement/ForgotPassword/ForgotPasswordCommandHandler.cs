using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Server.Domain.Auth.User;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.Server.Modules.Commands.Auth.PasswordManagement.ForgotPassword;

public record ForgotPasswordRequest(string Email, string Username) : IRequest;

public class ForgotPasswordCommandHandler(IScopedDbContext dbContext, IMessageProducer messageProducer) : IRequestHandler<ForgotPasswordRequest>
{
    public async Task Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Set<UserEntity>()
            .IgnoreQueryFilters()
            .Where(u => u.Email == request.Email && u.Username == request.Username)
            .Select(u => new
            {
                u.Id,
                u.PasswordResetConfirmation,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw ForgotPasswordInvalidFailure.Exception();

        if (user.PasswordResetConfirmation != null)
            dbContext.Remove(user.PasswordResetConfirmation);

        var newConfirmation = PasswordResetConfirmationEntity.Create(user.Id);
        dbContext.Add(newConfirmation);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new ForgotPasswordMessage(request.Username, request.Email, newConfirmation.Code), cancellationToken);
        }, cancellationToken);
    }
}
