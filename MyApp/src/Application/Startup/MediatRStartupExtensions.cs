using MediatR;
using MyApp.Application.Startup.Behaviours;

namespace MyApp.Application.Startup;

public static class MediatRStartupExtensions
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(config =>
         {
             config.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>();
             config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
         });

        return services;
    }
}
