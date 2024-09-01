using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Domain.Auth.EmailConfirmation.Failures;
using MyApp.Server.Infrastructure.Database;
using MyApp.Server.Modules.Commands.Auth.BackgroundJobs.ConfirmRegistration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Server.Modules.Commands.Auth.ResendConfirmation;

public record ResendConfirmationRequest(string Email) : IRequest;

public class ResendConfirmationCommandHandler(IScopedDbContext dbContext, IConfirmRegistrationEmailNotifier notifier) : IRequestHandler<ResendConfirmationRequest>
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

        await dbContext.SaveChangesAsync(cancellationToken);

        await notifier.StartInBackground(new(data.Username, request.Email, newConfirmation.Code), cancellationToken);
    }
}
