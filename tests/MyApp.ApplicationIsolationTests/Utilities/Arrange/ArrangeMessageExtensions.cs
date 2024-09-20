using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.ApplicationIsolationTests.Utilities.Arrange;

public static class ArrangeMessageExtensions
{
    public static void ArrangeMessageThrows<TMessage>(this MockBag mockBag) where TMessage : class
    {
        mockBag.Get<IMessageProducer>()
            .Setup(m => m.Send(It.IsAny<TMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
    }
}
