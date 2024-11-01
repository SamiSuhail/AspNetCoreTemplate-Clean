using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Presentation.Messaging.Queues;

public class SendChangeEmailConfirmationConsumer(ISender sender) : IConsumer<SendChangeEmailConfirmationMessage>
{
    public async Task Consume(ConsumeContext<SendChangeEmailConfirmationMessage> context)
        => await sender.Send(context.Message, context.CancellationToken);
}
