using MassTransit;
using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Infrastructure.Messaging;
using MyApp.Utilities.Settings;

namespace MyApp.Infrastructure.Startup;

public static class MessagingStartupExtensions
{
    public static IServiceCollection AddCustomMessaging<TPresentationAssemblyMarker>(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services.AddCustomSettings<MessagingSettings>(configuration);

        services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
                options.StopTimeout = TimeSpan.FromSeconds(30);
            });

        services.AddOptions<RabbitMqTransportOptions>()
            .BindConfiguration(MessagingSettings.SectionName);

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumersFromNamespaceContaining<TPresentationAssemblyMarker>();

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
