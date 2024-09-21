using System.Reflection;
using MyApp.Presentation.Endpoints.Core;

namespace MyApp.Presentation.Startup;

public static class EndpointsStartupExtensions
{
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