﻿using MyApp.Application.Infrastructure.Repositories.PasswordReset;
using MyApp.Application.Startup;

namespace Microsoft.Extensions.DependencyInjection;

public static class StartupExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddCustomMediatR()
            .AddCustomRequestTransformers()
            .AddCustomValidators()
            .AddTransient<IPasswordResetRepository, PasswordResetRepository>();

        return services;
    }
}
