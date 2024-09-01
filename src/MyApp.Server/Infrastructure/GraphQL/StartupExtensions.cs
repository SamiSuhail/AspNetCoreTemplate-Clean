using MyApp.Server.Modules.Queries;
using MyApp.Server.Modules.Queries.Auth;

namespace MyApp.Server.Infrastructure.GraphQL;

public static class StartupExtensions
{
    public static IServiceCollection AddCustomGraphQL(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<Query>()
            .AddTypeExtension<AuthQuery>()
            .AddErrorFilter<CustomErrorFilter>()
            .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = env.IsDevelopment());

        return services;
    }

    public static WebApplication MapCustomGraphQL(this WebApplication app)
    {
        app.MapGraphQL();

        return app;
    }
}
