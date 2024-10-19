using Microsoft.AspNetCore.Authorization;
using MyApp.Application.Infrastructure.Abstractions.Auth;

namespace MyApp.Infrastructure.Auth;

public record ScopeRequirement(string PolicyName) : IAuthorizationRequirement;

public class ScopeAuthorizationHandler(IUserContextAccessor userContextAccessor) : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        var policyName = requirement.PolicyName;

        var accessToken = userContextAccessor.AccessToken;

        if (accessToken.Scopes.Contains(policyName))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}