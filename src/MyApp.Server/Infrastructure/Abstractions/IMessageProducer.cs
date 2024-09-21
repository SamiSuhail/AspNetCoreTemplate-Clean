namespace MyApp.Server.Infrastructure.Abstractions;

public interface IMessageProducer
{
    Task Send<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class;
}
