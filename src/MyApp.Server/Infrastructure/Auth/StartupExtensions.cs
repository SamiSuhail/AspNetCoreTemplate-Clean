using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyApp.Server.Infrastructure.Auth;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = AuthSettings.Get(configuration);

        services.AddScoped<IJwtReader, JwtReader>();
        services.AddScoped<IUserContextAccessor, UserContextAccessor>();
        services.AddSingleton<IJwtGenerator, JwtGenerator>();
        services.AddSingleton(authSettings);
        services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(jwt =>
        {
            jwt.SaveToken = true;
            jwt.TokenValidationParameters = JwtValidationParametersProvider.Get(authSettings.Jwt.PublicKeyXml);
        });
        services.AddAuthorization();

        return services;
    }
}
