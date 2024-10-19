using System.Net;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Refit;

namespace MyApp.Tests.Utilities.Clients.Extensions;

public static class AssertResponseExtensions
{
    public static void AssertSuccess(this IApiResponse response)
    {
        using var _ = new AssertionScope();
        var intStatusCode = (int) response.StatusCode;
        intStatusCode.Should().BeGreaterThanOrEqualTo(200);
        intStatusCode.Should().BeLessThan(300);
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

    public static void AssertForbiddenError(this IApiResponse response)
        => response.AssertError(HttpStatusCode.Forbidden);

    public static void AssertUnauthorizedError(this IApiResponse response)
        => response.AssertError(HttpStatusCode.Unauthorized);

    public static void AssertError(this IApiResponse response, HttpStatusCode? statusCode = null)
    {
        using var _ = new AssertionScope();
        response.IsSuccessStatusCode.Should().BeFalse();
        response.Error?.Content.Should().NotBeNull();
        if (statusCode != null)
            response.StatusCode.Should().Be(statusCode);
    }
}
