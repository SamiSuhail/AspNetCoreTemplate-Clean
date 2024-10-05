namespace MyApp.Tests.Integration.Utilities.Assert;

public static class AssertGraphQLResponseExtensions
{
    public static void AssertSuccess(this IOperationResult response)
    {
        using var _ = new AssertionScope();
        response.Errors.Should().BeEmpty();
        response.Data.Should().NotBeNull();
    }

    public static void AssertUnauthorized(this IOperationResult response)
        => response.AssertSingleError("AUTH_NOT_AUTHORIZED", "The current user is not authorized to access this resource.");

    public static void AssertSingleError(this IOperationResult response, string errorCode, string message)
    {
        response.Errors.Should().HaveCount(1);
        var error = response.Errors[0];
        error.Should().NotBeNull();
        error.Code.Should().Be(errorCode);
        error.Message.Should().Be(message);
        response.Data.Should().BeNull();
    }
}
