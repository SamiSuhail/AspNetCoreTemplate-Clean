using MyApp.Server.Infrastructure.Messaging;

namespace MyApp.ApplicationIsolationTests.Utilities.Assert;

public static class AssertMessage
{
    public static void Produced<TMessage>(Times? times = null) where TMessage : class
    {
        MockBag.Get<IMessageProducer>()
            .Verify(x => x.Send(It.IsAny<TMessage>(), It.IsAny<CancellationToken>()),
                times ?? Times.Once());
    }
}
