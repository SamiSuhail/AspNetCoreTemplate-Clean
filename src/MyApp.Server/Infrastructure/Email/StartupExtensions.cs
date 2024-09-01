namespace MyApp.Server.Infrastructure.Email;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomEmail(this IServiceCollection services)
    {
        services.AddSingleton<IEmailSender, EmailSender>();
        return services;
    }
}
