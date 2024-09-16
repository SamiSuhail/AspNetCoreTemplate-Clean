﻿using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using MyApp.Server.Presentation.Startup.Filters;

namespace MyApp.Server.Presentation.Startup;

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
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApp.Server", Version = "v1" });
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

    public static IApplicationBuilder UseCustomSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}