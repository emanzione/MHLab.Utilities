using MHLab.Utilities.Communication;
using NUnit.Framework;

namespace MHLab.Utilities.Tests.Communication
{
    public readonly struct TestMessage : IMessage
    {
        public readonly int TestValue;

        public TestMessage(int testValue)
        {
            TestValue = testValue;
        }
    }

    public class TestMessageListener : IListener<TestMessage>
    {
        public int Counter { get; private set; }

        public void OnMessage(TestMessage message)
        {
            Counter++;
        }

        public void Dispose()
        {
        }
    }

    public class PubSubTest
    {
        private IPublisher               _publisher;
        private ISubscriber<TestMessage> _subscriber;

        [SetUp]
        public void Setup()
        {
            _publisher  = new Publisher();
            _subscriber = _publisher.GetSubscriber<TestMessage>();
        }

        [Test]
        public void Publish_Message_Correctly()
        {
            var listener = new TestMessageListener();
            _subscriber.Subscribe(listener);

            _publisher.Publish(new TestMessage(5));

            Assert.AreEqual(0, listener.Counter);

            _publisher.Consume();

            Assert.AreEqual(1, listener.Counter);
        }

        [Test]
        public void Message_Is_Correctly_Delivered_Just_Once()
        {
            var listener = new TestMessageListener();
            _subscriber.Subscribe(listener);

            _publisher.Publish(new TestMessage(5));

            Assert.AreEqual(0, listener.Counter);

            _publisher.Consume();

            Assert.AreEqual(1, listener.Counter);

            _publisher.Consume();

            Assert.AreEqual(1, listener.Counter);
        }

        [Test]
        public void Publish_Correctly_To_All_Subscribers()
        {
            var listener1 = new TestMessageListener();
            var listener2 = new TestMessageListener();

            _subscriber.Subscribe(listener1);
            _subscriber.Subscribe(listener2);

            _publisher.Publish(new TestMessage(5));

            Assert.AreEqual(0, listener1.Counter);
            Assert.AreEqual(0, listener2.Counter);

            _publisher.Consume();

            Assert.AreEqual(1, listener1.Counter);
            Assert.AreEqual(1, listener2.Counter);
        }

        [Test]
        public void Unsubscribe_Correctly()
        {
            var listener = new TestMessageListener();
            _subscriber.Subscribe(listener);

            _publisher.Publish(new TestMessage(5));

            Assert.AreEqual(0, listener.Counter);

            _publisher.Consume();

            Assert.AreEqual(1, listener.Counter);

            _subscriber.Unsubscribe(listener);

            _publisher.Publish(new TestMessage(5));

            _publisher.Consume();

            Assert.AreEqual(1, listener.Counter);
        }
    }
}