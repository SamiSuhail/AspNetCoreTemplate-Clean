using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.EmailConfirmation.Failures;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Infrastructure.Messaging;
using MyApp.Server.Modules.Commands.Auth.ConfirmRegistration;

namespace MyApp.Server.Modules.Commands.Auth.ResendConfirmation;

public record ResendConfirmationRequest(string Email) : IRequest;

public class ResendConfirmationCommandHandler(IScopedDbContext dbContext, IMessageProducer messageProducer) : IRequestHandler<ResendConfirmationRequest>
{
    public async Task Handle(ResendConfirmationRequest request, CancellationToken cancellationToken)
    {
        var data = await dbContext.Set<EmailConfirmationEntity>()
            .IgnoreQueryFilters()
            .Where(ec => ec.User.Email == request.Email && ec.User.IsEmailConfirmed == false)
            .Select(ec => new
            {
                EmailConfirmation = ec,
                ec.User.Username,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw ResendConfirmationInvalidFailure.Exception();

        dbContext.Remove(data.EmailConfirmation);
        var newConfirmation = EmailConfirmationEntity.Create(data.EmailConfirmation.UserId);
        dbContext.Add(newConfirmation);

        await dbContext.WrapInTransaction(async () =>
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await messageProducer.Send(new ConfirmRegistrationMessage(data.Username, request.Email, newConfirmation.Code), cancellationToken);
        }, cancellationToken);
    }
}
