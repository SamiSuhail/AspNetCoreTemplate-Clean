using MyApp.Server.Presentation.GraphQL;
using MyApp.Server.Presentation.Startup.Middleware;

namespace MyApp.Server.Presentation.Startup;

public static class GraphQLStartupExtensions
{
    public static IServiceCollection AddCustomGraphQL(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<Query>()
            .AddTypeExtension<AuthQuery>()
            .AddErrorFilter<CustomGraphQLExceptionHandler>()
            .AddTypeConverter<DateTime, DateTimeOffset>(dateTime => new DateTimeOffset(dateTime, TimeSpan.Zero))
            .AddTypeConverter<DateTimeOffset, DateTime>(dateTimeOffset => dateTimeOffset.UtcDateTime)
            .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = env.IsDevelopment());

        return services;
    }

    public static WebApplication MapCustomGraphQL(this WebApplication app)
    {
        app.MapGraphQL();

        return app;
    }
}
