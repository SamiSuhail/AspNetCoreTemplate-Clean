using System.Reflection;
using MediatR;
using MyApp.Application.Startup.Behaviours;

namespace MyApp.Application.Startup;

public static class MediatRStartupExtensions
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(config =>
         {
             config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
             config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
         });

        return services;
    }
}
