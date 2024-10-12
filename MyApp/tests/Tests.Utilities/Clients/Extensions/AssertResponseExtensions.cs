﻿using System.Net;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using MyApp.Domain.Auth.User.Failures;
using Newtonsoft.Json;
using Refit;

namespace MyApp.Tests.Utilities.Clients.Extensions;

public static class AssertResponseExtensions
{
    public static void AssertSuccess(this IApiResponse response)
    {
        using var _ = new AssertionScope();
        response.IsSuccessStatusCode.Should().BeTrue();
        response.Error?.Content.Should().BeNull();
    }

    public static void AssertInvalidLoginFailure(this IApiResponse response)
        => response.AssertSingleBadRequestError(LoginInvalidFailure.Key, LoginInvalidFailure.Message);

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
