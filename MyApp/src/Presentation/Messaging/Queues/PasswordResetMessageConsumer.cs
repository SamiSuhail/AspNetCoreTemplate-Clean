using MyApp.Presentation.Interfaces.Messaging;

namespace MyApp.Presentation.Messaging.Queues;

public class PasswordResetSendConfirmationMessageConsumer(ISender sender) : IConsumer<PasswordResetSendConfirmationMessage>
{
    public async Task Consume(ConsumeContext<PasswordResetSendConfirmationMessage> context)
        => await sender.Send(context.Message, context.CancellationToken);
}
