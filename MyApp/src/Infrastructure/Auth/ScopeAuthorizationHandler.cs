using Microsoft.AspNetCore.Authorization;
using MyApp.Application.Infrastructure.Abstractions.Auth;

namespace MyApp.Infrastructure.Auth;

public record ScopeRequirement(string PolicyName) : IAuthorizationRequirement;

public class ScopeAuthorizationHandler(
    IUserContextAccessor userContextAccessor,
    ILogger<ScopeAuthorizationHandler> logger
    ) : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        var policyName = requirement.PolicyName;
        var userData = userContextAccessor.UserData;

        if (userData.Scopes.Contains(policyName))
            context.Succeed(requirement);
        else
        {
            context.Fail(new(this, $"Required policy {policyName} is not available for this user. Please try re-logging into your account to generate a new token."));
            logger.Verbose("Required policy {PolicyName} failed for user.", policyName);
        }

        return Task.CompletedTask;
    }
}