using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.PasswordResetConfirmation;
using MyApp.Domain.Auth.PasswordResetConfirmation.Failures;
using MyApp.Domain.Auth.User;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.PasswordManagement.ForgotPassword;

namespace MyApp.Application.Handlers.Commands.Auth.PasswordManagement.ForgotPassword;

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
