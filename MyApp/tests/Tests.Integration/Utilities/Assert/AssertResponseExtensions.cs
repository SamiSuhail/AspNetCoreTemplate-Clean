using MyApp.Presentation.Startup.Filters;
using MyApp.Presentation.Startup.Middleware;
using MyApp.Tests.Utilities.Clients.Extensions;

namespace MyApp.Tests.Integration.Utilities.Assert;

public static class AssertResponseExtensions
{
    public static void AssertAnonymousOnlyError(this IApiResponse response)
        => response.AssertSingleBadRequestError(AnonymousOnlyConstants.Key, AnonymousOnlyConstants.Message);

    public static void AssertInternalServerError(this IApiResponse response)
    {
        response.AssertError(HttpStatusCode.InternalServerError);
        response.Error!.Content.Should().Be(UnhandledExceptionConstants.Message);
    }
}
