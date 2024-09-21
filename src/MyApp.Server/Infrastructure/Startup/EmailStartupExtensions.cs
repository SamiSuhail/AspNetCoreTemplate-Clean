using MyApp.Server.Infrastructure.Email;

namespace MyApp.Server.Infrastructure.Startup;

public static class EmailStartupExtensions
{
    public static IServiceCollection AddCustomEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomSettings<EmailSettings>(configuration);
        services.AddSingleton<IEmailSender, EmailSender>();
        return services;
    }
}
