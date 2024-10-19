using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Domain.Access.Scope;
using MyApp.Infrastructure.Auth;
using MyApp.Utilities.Settings;

namespace MyApp.Infrastructure.Startup;

public static class AuthStartupExtensions
{
    public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = services.AddCustomSettings<AuthSettings>(configuration);

        services.AddSingleton<IJwtGenerator, JwtGenerator>();
        services.AddSingleton<IJwtReader, JwtReader>();

        services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(jwt =>
        {
            jwt.SaveToken = true;
            jwt.MapInboundClaims = false;
            jwt.TokenValidationParameters = JwtValidationParametersProvider.Get(authSettings.Jwt.PublicKeyXml);
        });

        services.AddScoped<IAuthorizationHandler, ScopeAuthorizationHandler>();
        services.AddAuthorization(options =>
        {
            foreach (var scopeName in CustomScopes.All)
                options.AddPolicy(scopeName, policy => policy.Requirements.Add(new ScopeRequirement(scopeName)));
        });

        return services;
    }
}
