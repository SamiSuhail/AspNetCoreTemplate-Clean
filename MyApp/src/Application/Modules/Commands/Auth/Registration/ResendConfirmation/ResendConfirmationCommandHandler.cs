using MediatR;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Application.Infrastructure.Abstractions.Database;
using MyApp.Domain.Auth.UserConfirmation;
using MyApp.Domain.Auth.UserConfirmation.Failures;
using MyApp.Presentation.Interfaces.Http.Commands.Auth.Registration.ResendConfirmation;
using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Application.Modules.Commands.Auth.Registration.ResendConfirmation;

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
            await messageProducer.Send(new SendRegisterUserConfirmationMessage(data.Username, request.Email, newConfirmation.Code), cancellationToken);
        }, cancellationToken);
    }
}
