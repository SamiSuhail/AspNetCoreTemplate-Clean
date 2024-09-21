using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyApp.Server.Infrastructure.Startup;

namespace MyApp.Server.Infrastructure.Auth;

public static class AuthStartupExtensions
{
    public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = services.AddCustomSettings<AuthSettings>(configuration);

        services.AddScoped<IJwtReader, JwtReader>();
        services.AddScoped<IUserContextAccessor, UserContextAccessor>();
        services.AddSingleton<IJwtGenerator, JwtGenerator>();

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
        services.AddAuthorization();

        return services;
    }
}
