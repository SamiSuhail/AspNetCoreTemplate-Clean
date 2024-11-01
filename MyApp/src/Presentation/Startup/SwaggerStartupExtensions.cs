using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using MyApp.Presentation.Startup.Filters;

namespace MyApp.Presentation.Startup;

public static class SwaggerStartupExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddFluentValidationRulesToSwagger();
        services.AddSwaggerGen(options =>
        {
            options.SupportNonNullableReferenceTypes();
            options.SchemaFilter<RequiredNotNullableSwaggerSchemaFilter>();
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApp", Version = "v1" });
            const string SecurityDefinitionName = "JWT";
            options.AddSecurityDefinition(SecurityDefinitionName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Use /api/auth/login to get an access token. Type into the textbox below: Bearer {your JWT token}.",
                Scheme = "ApiKeyScheme",
            });

            var key = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = SecurityDefinitionName,
                },
                In = ParameterLocation.Header
            };

            var requirement = new OpenApiSecurityRequirement
            {
                [key] = [],
            };
            options.AddSecurityRequirement(requirement);
        });
        return services;
    }

    public static WebApplication UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
