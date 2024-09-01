using FluentValidation;

namespace MyApp.Server.Infrastructure.Validation;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ProgramApi>(lifetime: ServiceLifetime.Transient);

        return services;
    }
}
