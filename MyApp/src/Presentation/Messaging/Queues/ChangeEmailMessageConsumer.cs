using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Presentation.Messaging.Queues;

internal class ChangeEmailMessageConsumer(ISender sender) : IConsumer<ChangeEmailMessage>
{
    public async Task Consume(ConsumeContext<ChangeEmailMessage> context)
        => await sender.Send(context.Message, context.CancellationToken);
}
