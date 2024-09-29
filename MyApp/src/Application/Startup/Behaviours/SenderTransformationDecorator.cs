using MediatR;
using MyApp.Application.Utilities;

namespace MyApp.Application.Startup.Behaviours;

public class SenderTransformationDecorator(
    ISender sender,
    IServiceProvider services)
    : ISender
{
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        => sender.CreateStream(request, cancellationToken);

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        => sender.CreateStream(request, cancellationToken);

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var transformer = services.GetService<IRequestTransformer<IRequest<TResponse>>>();

        if (transformer == null)
            return sender.Send(request, cancellationToken);

        var transformedRequest = transformer.Transform(request);
        return sender.Send(transformedRequest, cancellationToken);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        var transformer = services.GetService<IRequestTransformer<TRequest>>();

        if (transformer == null)
            return sender.Send(request, cancellationToken);

        var transformedRequest = transformer.Transform(request);
        return sender.Send(transformedRequest, cancellationToken);
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        => sender.Send(request, cancellationToken);
}
