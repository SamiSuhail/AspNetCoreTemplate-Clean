using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Presentation.Messaging.Queues;

public class SendPasswordResetConfirmationConsumer(ISender sender) : IConsumer<SendPasswordResetConfirmationMessage>
{
    public async Task Consume(ConsumeContext<SendPasswordResetConfirmationMessage> context)
        => await sender.Send(context.Message, context.CancellationToken);
}
