using MyApp.Server.Domain.Auth.PasswordResetConfirmation;
using MyApp.Server.Infrastructure.Email;
using Quartz;

namespace MyApp.Server.Modules.Commands.Auth.ForgotPassword.EmailNotifier;

public class ForgotPasswordBackgroundJob(IEmailSender emailSender) : IJob
{
    public const string MessageTemplate = """
        Please use the code below to reset your password. This code is valid for {0} minutes after time of requesting it. <br />
        Code: {1}
        """;

    public async Task Execute(IJobExecutionContext context)
    {
        var jobData = context.JobDetail.JobDataMap;

        var username = jobData.GetString(nameof(ForgotPasswordEmailNotifierRequest.Username))!;
        var email = jobData.GetString(nameof(ForgotPasswordEmailNotifierRequest.Email))!;
        var code = jobData.GetString(nameof(ForgotPasswordEmailNotifierRequest.Code))!;

        var messageText = string.Format(MessageTemplate, PasswordResetConfirmationConstants.ExpirationTimeMinutes, code);
        await emailSender.Send(username, email, "Go2Gether Password Reset", messageText, context.CancellationToken);
    }
}
