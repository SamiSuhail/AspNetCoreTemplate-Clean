using System.Net;
using FluentAssertions;
using Refit;

namespace MyApp.ApplicationIsolationTests.Core;

public static class AssertionExtensions
{
    public static void AssertStatusCode(this IApiResponse response, HttpStatusCode? statusCode = null)
    {
        if (response.StatusCode == statusCode)
        {
            return;
        }

        if (statusCode == null && response.IsSuccessStatusCode)
        {
            return;
        }

        var errorMessage = response.Error?.Content;
        throw new Exception($"Expected {statusCode?.ToString() ?? "success" } but found {response.StatusCode}{(errorMessage == null ? string.Empty : $" with error message: {errorMessage}")}.");
    }

    public static void ShouldBeToday(this DateTimeOffset date)
        => DateOnly.FromDateTime(date.Date).Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
}
