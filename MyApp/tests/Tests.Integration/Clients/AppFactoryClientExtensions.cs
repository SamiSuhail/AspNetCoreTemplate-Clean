﻿using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Domain.Access.Scope;

namespace MyApp.Tests.Integration.Clients;

public static class AppFactoryClientExtensions
{
    public static IApplicationAdminClient ArrangeClientWithCredentials(
        this AppFactory appFactory,
        int userId,
        string username,
        string email)
    {
        var jwtGenerator = appFactory.Services.GetRequiredService<IJwtGenerator>();
        var accessToken = jwtGenerator.CreateAccessToken(userId, username, email, ScopeCollection.Empty);
        return appFactory.ArrangeClientWithToken(accessToken);
    }

    public static IApplicationAdminClient ArrangeClientWithToken(this AppFactory appFactory, string accessToken)
    {
        var httpClient = appFactory.CreateClient().SetAuthorizationHeader(accessToken);
        return RestService.For<IApplicationAdminClient>(httpClient);
    }

    public static IApplicationGraphQLClient ArrangeGraphQLClientWithToken(this AppFactory appFactory, string accessToken)
        => appFactory.CreateGraphQLClient(c => c.SetAuthorizationHeader(accessToken));
}
