using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Auth.UserConfirmation.Failures;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Application.Interfaces.Commands.Auth.Registration.ResendConfirmation;

namespace MyApp.Application.Handlers.Commands.Auth.Registration.ResendConfirmation;

public class ResendConfirmationCommandHandler(IScopedDbContext dbContext, IMessageProducer messageProducer) : IRequestHandler<ResendConfirmationRequest>
{
    public async Task Handle(ResendConfirmationRequest request, CancellationToken cancellationToken)
    {
        var data = await dbContext.Set<UserConfirmationEntity>()
            .IgnoreQueryFilters()
            .Where(uc => uc.User.Email == request.Email && uc.User.IsEmailConfirmed == false)
            .Select(uc => new
            {
                UserConfirmation = uc,
                uc.User.Username,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw ResendConfirmationInvalidFailure.Exception();

        dbContext.Remove(data.UserConfirmation);
        var newConfirmation = UserConfirmationEntity.Create(data.UserConfirmation.UserId);
        dbContext.Add(newConfirmation);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new SendUserConfirmationMessage(data.Username, request.Email, newConfirmation.Code), cancellationToken);
        }, cancellationToken);
    }
}
