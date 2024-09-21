using System.Collections.Concurrent;
using MassTransit;
using MyApp.Server.Infrastructure.Abstractions;
using MyApp.Server.Shared;

namespace MyApp.Server.Infrastructure.Messaging;

public class MessageProducer(ISendEndpointProvider bus) : IMessageProducer
{
    private static readonly ConcurrentDictionary<Type, Uri> _uris = [];

    public async Task Send<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class
    {
        var uri = GetDefaultUri<TMessage>();
        var endpoint = await bus.GetSendEndpoint(uri);
        await endpoint.Send(message, cancellationToken);
    }

    private static Uri GetDefaultUri<TMessage>()
    {
        if (_uris.TryGetValue(typeof(TMessage), out var uri))
            return uri;

        var cleanedName = typeof(TMessage).Name.Replace("Message", string.Empty);
        var kebabCaseName = cleanedName.PascalToKebabCase();
        var newUri = new Uri($"queue:{kebabCaseName}");
        _uris.TryAdd(typeof(TMessage), newUri);
        return newUri;
    }
}
