using MHLab.Utilities.Messaging;
using NUnit.Framework;

namespace MHLab.Utilities.Tests.MessagingTests
{
    public class PublisherTests
    {
        private IPublisher<IMyMessage>      _publisher;
        private IMessageHistory<IMyMessage> _messageHistory;
        private MyMessageHandler            _handler;
        private MyMessageHandler            _handler2;

        [SetUp]
        public void Setup()
        {
            _publisher      = new Publisher<IMyMessage>();
            _messageHistory = new MyMessageHistory();
            _handler        = new MyMessageHandler();
            _handler2       = new MyMessageHandler();
        }

        [Test]
        public void Publish_Message_Correctly()
        {
            _publisher.Subscribe(_handler);
            Assert.AreEqual(0, _handler.Counter);

            _publisher.Publish(new TestMessage(5));

            // Without calling Deliver, messages shouldn't be delivered.
            Assert.AreEqual(0, _handler.Counter);

            _publisher.Deliver(_messageHistory);

            Assert.AreEqual(5, _handler.Counter);
        }

        [Test]
        public void Message_Is_Correctly_Delivered_Just_Once()
        {
            _publisher.Subscribe(_handler);

            _publisher.Publish(new TestMessage(1));
            _publisher.Deliver(_messageHistory);

            Assert.AreEqual(1, _handler.Counter);

            _publisher.Deliver(_messageHistory);

            Assert.AreEqual(1, _handler.Counter);
        }

        [Test]
        public void Publish_Correctly_To_All_Subscribers()
        {
            _publisher.Subscribe(_handler);
            _publisher.Subscribe(_handler2);

            _publisher.Publish(new TestMessage(5));

            Assert.AreEqual(0, _handler.Counter);
            Assert.AreEqual(0, _handler2.Counter);

            _publisher.Deliver(_messageHistory);

            Assert.AreEqual(5, _handler.Counter);
            Assert.AreEqual(5, _handler2.Counter);
        }

        [Test]
        public void Unsubscribe_Correctly()
        {
            var subscription = _publisher.Subscribe(_handler);
            _publisher.Publish(new TestMessage(8));
            _publisher.Deliver(_messageHistory);

            Assert.AreEqual(8, _handler.Counter);

            _publisher.Unsubscribe(subscription);

            _publisher.Publish(new TestMessage(5));
            _publisher.Deliver(_messageHistory);

            Assert.AreEqual(8, _handler.Counter);
        }

        [Test]
        public void History_Correctly_Updated()
        {
            _publisher.Subscribe(_handler);
            _publisher.Publish(new TestMessage(8));
            _publisher.Deliver(_messageHistory);

            Assert.AreEqual(1, _messageHistory.Size);
        }
    }
}