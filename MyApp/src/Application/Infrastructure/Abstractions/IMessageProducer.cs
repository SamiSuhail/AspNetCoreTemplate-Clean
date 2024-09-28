namespace MyApp.Application.Infrastructure.Abstractions;

public interface IMessageProducer
{
    Task Send<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class;
}
