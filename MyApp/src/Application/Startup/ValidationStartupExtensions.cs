using FluentValidation;

namespace MyApp.Application.Startup;

public static class ValidationStartupExtensions
{
    public static IServiceCollection AddCustomValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>(lifetime: ServiceLifetime.Transient);

        return services;
    }
}
