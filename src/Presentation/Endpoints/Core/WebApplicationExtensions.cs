using MyApp.Application.Utilities;

namespace MyApp.Presentation.Endpoints.Core;

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
}