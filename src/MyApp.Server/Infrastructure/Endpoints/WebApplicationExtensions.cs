using System.Reflection;
using MyApp.Server.Utilities;

namespace MyApp.Server.Infrastructure.Endpoints;

public static class WebApplicationExtensions
{
    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
    {
        var groupName = group.GetType().Name;

        return app
            .MapGroup($"/api/{groupName.PascalToKebabCase()}")
            .WithTags(groupName)
            .WithOpenApi()
            .RequireAuthorization();
    }

    public static WebApplication MapCustomEndpoints(this WebApplication app)
    {
        app.Map("/", () => Results.Redirect("/api"));
        var endpointGroupType = typeof(EndpointGroupBase);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }
}