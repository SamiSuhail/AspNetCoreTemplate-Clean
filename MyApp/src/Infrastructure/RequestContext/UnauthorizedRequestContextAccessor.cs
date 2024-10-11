using MyApp.Application.Infrastructure.Abstractions.Auth;
using MyApp.Domain.Infra.Instance;
using MyApp.Utilities.Collections;

namespace MyApp.Infrastructure.RequestContext;

public class UnauthorizedRequestContextAccessor : IUnauthorizedRequestContextAccessor
{
    const string CustomHeadersPrefix = "Custom-";
    const string InstanceNameHeaderKey = $"{CustomHeadersPrefix}{nameof(InstanceName)}";

    public UnauthorizedRequestContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        var headers = httpContextAccessor.HttpContext?.Request?.Headers
            ?? throw new InvalidOperationException("No request headers accessible in scope.");
        InstanceName = headers.GetValueOrDefault(InstanceNameHeaderKey, InstanceConstants.DefaultInstanceName)!;
    }

    public string InstanceName { get; set; }
}
