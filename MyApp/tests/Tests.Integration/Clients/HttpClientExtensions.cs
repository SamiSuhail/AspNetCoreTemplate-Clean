using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyApp.Tests.Integration.Clients;

public static class HttpClientExtensions
{
    public static HttpClient SetAuthorizationHeader(this HttpClient httpClient, string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new(JwtBearerDefaults.AuthenticationScheme, accessToken);
        return httpClient;
    }
}
