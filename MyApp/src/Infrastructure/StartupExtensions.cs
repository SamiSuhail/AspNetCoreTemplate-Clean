﻿using MyApp.Application.Infrastructure.Abstractions;
using MyApp.Infrastructure;
using MyApp.Infrastructure.Database;
using MyApp.Infrastructure.Startup;
using MyApp.Infrastructure.Utilities;

namespace Microsoft.Extensions.DependencyInjection;

public static class StartupExtensions
{
    public static IServiceCollection AddInfrastructureServices<TPresentationAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IClock, Clock>()
            .AddCustomHttpContextAccessors()
            .AddCustomAuth(configuration)
            .AddCustomDatabase(configuration)
            .AddCustomEmail(configuration)
            .AddCustomMessaging<TPresentationAssemblyMarker>(configuration)
            .AddCustomLogging(configuration)
            .AddCustomBackgroundJobs(configuration);

        return services;
    }

    public static IConfigurationBuilder AddInfrastructureAppsettings(this IConfigurationBuilder builder)
    {
        const string FileName = "appsettings.infrastructure.json";

        var directory = Directory.GetParent(typeof(IInfrastructureAssemblyMarker).Assembly.Location)?.FullName
            ?? throw new Exception($"Could not find assembly for type {nameof(IInfrastructureAssemblyMarker)} when trying to load configuration for file {FileName}");

        var path = $"{directory}/{FileName}";

        builder.AddJsonFile(path, optional: false, reloadOnChange: true);

        return builder;
    }

    public static WebApplication UseCustomInfrastructure(this WebApplication app)
    {
        app.UseCustomSerilog()
            .UseAuthentication()
            .UseAuthorization();

        return app;
    }

    public static async Task SeedDatabase(this WebApplication app)
    {
        var seeder = app.Services.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedDatabase();
    }
}
