using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Presentation.Messaging.Queues;

public class SendUserConfirmationMessageConsumer(ISender sender) : IConsumer<SendUserConfirmationMessage>
{
    public async Task Consume(ConsumeContext<SendUserConfirmationMessage> context)
        => await sender.Send(context.Message, context.CancellationToken);
}
