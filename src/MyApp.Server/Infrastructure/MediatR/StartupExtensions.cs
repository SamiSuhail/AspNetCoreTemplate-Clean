using System.Reflection;
using MyApp.Server.Infrastructure.MediatR.Behaviours;
using MediatR;

namespace MyApp.Server.Infrastructure.MediatR;

public static class StartupExtensions
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
