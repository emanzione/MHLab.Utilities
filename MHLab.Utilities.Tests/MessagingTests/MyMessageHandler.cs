using MHLab.Utilities.Messaging;

namespace MHLab.Utilities.Tests.MessagingTests
{
    public class MyMessageHandler : IMessageHandler<TestMessage, IMyMessage>
    {
        public int Counter { get; private set; }

        public void OnMessageDelivered(TestMessage message)
        {
            Counter += message.Counter;
        }

        public void Dispose()
        {
        }
    }
}