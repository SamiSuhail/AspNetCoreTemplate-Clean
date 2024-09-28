using MyApp.Application.Infrastructure.Abstractions;

namespace MyApp.ApplicationIsolationTests.Utilities.Assert;

public static class AssertMessageExtensions
{
    public static void AssertProduced<TMessage>(this MockBag mockBag, Times? times = null) where TMessage : class
    {
        mockBag.Get<IMessageProducer>()
            .Verify(x => x.Send(It.IsAny<TMessage>(), It.IsAny<CancellationToken>()),
                times ?? Times.Once());
    }
}
