using MediatR;
using Quartz;

namespace MyApp.Server.Modules.Commands.Auth.ForgotPassword.EmailNotifier;

public record ForgotPasswordEmailNotifierRequest(string Username, string Email, string Code);

public interface IForgotPasswordEmailNotifier
{
    Task StartInBackground(ForgotPasswordEmailNotifierRequest input, CancellationToken cancellationToken);
}

public class ForgotPasswordEmailNotifier(ISchedulerFactory schedulerFactory) : IForgotPasswordEmailNotifier
{
    public async Task StartInBackground(ForgotPasswordEmailNotifierRequest request, CancellationToken cancellationToken)
    {
        var jobKey = JobKey.Create(nameof(ForgotPasswordBackgroundJob));

        var job = JobBuilder.Create<ForgotPasswordBackgroundJob>()
            .WithIdentity(jobKey)
            .UsingJobData(nameof(request.Username), request.Username)
            .UsingJobData(nameof(request.Email), request.Email)
            .UsingJobData(nameof(request.Code), request.Code)
            .RequestRecovery()
            .Build();

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .StartNow()
            .Build();

        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}
