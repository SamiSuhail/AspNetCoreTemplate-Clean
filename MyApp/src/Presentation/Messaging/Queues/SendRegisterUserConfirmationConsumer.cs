using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Presentation.Messaging.Queues;

public class SendRegisterUserConfirmationConsumer(ISender sender) : IConsumer<SendRegisterUserConfirmationMessage>
{
    public async Task Consume(ConsumeContext<SendRegisterUserConfirmationMessage> context)
        => await sender.Send(context.Message, context.CancellationToken);
}
