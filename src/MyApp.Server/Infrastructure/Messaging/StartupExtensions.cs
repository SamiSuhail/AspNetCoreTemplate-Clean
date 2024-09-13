using System.Reflection;
using MassTransit;
using MyApp.Server.Modules.Commands.Auth.ConfirmRegistration;

namespace MyApp.Server.Infrastructure.Messaging;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = MessagingSettings.Get(configuration);
        services.AddSingleton(settings);

        services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                options.WaitUntilStarted = true;
            });

        services.AddOptions<RabbitMqTransportOptions>()
            .BindConfiguration(MessagingSettings.SectionName);

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            x.AddConfigureEndpointsCallback((name, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(3)));
            });
        });

        services.AddScoped<IMessageProducer, MessageProducer>();

        return services;
    }
}
