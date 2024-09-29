using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Infrastructure.Email;

namespace MyApp.Infrastructure.Startup;

public static class EmailStartupExtensions
{
    public static IServiceCollection AddCustomEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomSettings<EmailSettings>(configuration);
        services.AddSingleton<IEmailSender, EmailSender>();
        return services;
    }
}
