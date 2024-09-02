namespace MyApp.Server.Infrastructure.Email;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomEmail(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = EmailSettings.Get(configuration);

        services.AddSingleton(settings);
        services.AddSingleton<IEmailSender, EmailSender>();
        return services;
    }
}
