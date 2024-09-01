using MyApp.Server.Domain.Auth.EmailConfirmation;
using MyApp.Server.Infrastructure.Email;
using Quartz;

namespace MyApp.Server.Modules.Commands.Auth.BackgroundJobs.ConfirmRegistration;

public class ConfirmRegsitrationBackgroundJob(IEmailSender emailSender) : IJob
{
    public const string MessageTemplate = """
        Please use the code below to confirm your e-mail address. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public async Task Execute(IJobExecutionContext context)
    {
        var jobData = context.JobDetail.JobDataMap;

        var username = jobData.GetString(nameof(ConfirmRegistrationEmailNotifierRequest.Username))!;
        var email = jobData.GetString(nameof(ConfirmRegistrationEmailNotifierRequest.Email))!;
        var code = jobData.GetString(nameof(ConfirmRegistrationEmailNotifierRequest.Code))!;

        var messageText = string.Format(MessageTemplate, EmailConfirmationConstants.ExpirationTimeMinutes, code);
        await emailSender.Send(username, email, "Go2Gether Registration Confirmation", messageText, context.CancellationToken);
    }
}
