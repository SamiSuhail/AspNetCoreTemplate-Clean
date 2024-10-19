using MyApp.Domain.Auth.User.Failures;
using MyApp.Presentation.Startup.Filters;
using MyApp.Presentation.Startup.Middleware;

namespace MyApp.Tests.Integration.Utilities.Assert;

public static class AssertResponseExtensions
{
    public static void AssertInvalidLoginFailure(this IApiResponse response)
        => response.AssertSingleBadRequestError(LoginInvalidFailure.Key, LoginInvalidFailure.Message);

    public static void AssertAnonymousOnlyError(this IApiResponse response)
        => response.AssertSingleBadRequestError(AnonymousOnlyConstants.Key, AnonymousOnlyConstants.Message);

    public static void AssertInternalServerError(this IApiResponse response)
    {
        response.AssertError(HttpStatusCode.InternalServerError);
        response.Error!.Content.Should().Be(UnhandledExceptionConstants.Message);
    }

    public static void AssertUserNotFoundFailure(this IApiResponse response)
        => response.AssertSingleBadRequestError(UserNotFoundFailure.Key, UserNotFoundFailure.Message);

    public static void AssertUserNotFoundFailure(this IOperationResult response)
        => response.AssertSingleError(UserNotFoundFailure.Key, UserNotFoundFailure.Message);
}
