﻿using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyApp.Tests.Utilities.Clients.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient SetAuthorizationHeader(this HttpClient httpClient, string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new(JwtBearerDefaults.AuthenticationScheme, accessToken);
        return httpClient;
    }
}
