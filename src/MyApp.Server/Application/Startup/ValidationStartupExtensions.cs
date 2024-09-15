using FluentValidation;

namespace MyApp.Server.Application.Startup;

public static class ValidationStartupExtensions
{
    public static IServiceCollection AddCustomValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ProgramApi>(lifetime: ServiceLifetime.Transient);

        return services;
    }
}
