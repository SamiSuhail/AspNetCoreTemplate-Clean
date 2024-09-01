using Quartz;

namespace MyApp.Server.Modules.Commands.Auth.BackgroundJobs.ConfirmRegistration;

public record ConfirmRegistrationEmailNotifierRequest(string Username, string Email, string Code);

public interface IConfirmRegistrationEmailNotifier
{
    Task StartInBackground(ConfirmRegistrationEmailNotifierRequest input, CancellationToken cancellationToken);
}

public class ConfirmRegistrationEmailNotifier(ISchedulerFactory schedulerFactory) : IConfirmRegistrationEmailNotifier
{
    public async Task StartInBackground(ConfirmRegistrationEmailNotifierRequest request, CancellationToken cancellationToken)
    {
        var jobKey = JobKey.Create(nameof(ConfirmRegsitrationBackgroundJob));

        var job = JobBuilder.Create<ConfirmRegsitrationBackgroundJob>()
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
