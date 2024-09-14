using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MyApp.ApplicationIsolationTests.Utilities;

public static class AssertionExtensions
{
    public static void AssertSuccess(this IApiResponse response)
    {
        using var _ = new AssertionScope();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.Error?.Content.Should().BeNull();
    }
    public static void AssertSingleBadRequestError(this IApiResponse response, string key, string message)
    {
        response.AssertBadRequestErrors(new()
        {
            { key, [message] }
        });
    }

    public static void AssertBadRequestErrors(this IApiResponse response, Dictionary<string, string[]> expectedErrors)
    {
        var problemDetails = response.AssertBadRequest();
        try
        {
            problemDetails.Errors.Should().BeEquivalentTo(expectedErrors);
        }
        catch (Exception)
        {
            throw new Exception($"""

                Expected error:
                {JsonConvert.SerializeObject(expectedErrors, Formatting.Indented)}
                Instead found:
                {JsonConvert.SerializeObject(problemDetails.Errors, Formatting.Indented)}
                """);
        }
    }

    public static ValidationProblemDetails AssertBadRequest(this IApiResponse response)
    {
        response.AssertError(HttpStatusCode.BadRequest);
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(response.Error!.Content!)!;
        return problemDetails;
    }

    public static void AssertError(this IApiResponse response, HttpStatusCode? statusCode = null)
    {
        using var _ = new AssertionScope();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.Error?.Content.Should().NotBeNull();
        if (statusCode != null)
            response.StatusCode.Should().Be(statusCode);
    }

    public static void AssertSuccess(this IOperationResult response)
    {
        using var _ = new AssertionScope();
        response.Errors.Should().BeEmpty();
        response.Data.Should().NotBeNull();
    }

    public static void AssertUnauthorized(this IOperationResult response)
        => response.AssertSingleError("AUTH_NOT_AUTHORIZED");

    public static void AssertSingleError(this IOperationResult response, string errorCode)
    {
        response.Errors.Should().HaveCount(1);
        var error = response.Errors[0];
        error.Should().NotBeNull();
        error.Code.Should().Be(errorCode);
        response.Data.Should().BeNull();
    }

    public static void ShouldBeNow(this DateTime date, int precisionSeconds = 10)
        => date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(precisionSeconds));
}
